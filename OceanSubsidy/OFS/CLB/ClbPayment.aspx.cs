using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.IO;
using GS.OCA_OceanSubsidy.Entity;

public partial class OFS_CLB_ClbPayment : System.Web.UI.Page
{
    /// <summary>
    /// 目前處理的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitializePage();
        }
    }
    
    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 設定 Master Page 的計畫資訊
            var master = Master as OFS_CLB_ClbInprogress;
            if (master != null && !string.IsNullOrEmpty(ProjectID))
            {
                master.ProjectInfoText = $"{ProjectID}/請款核銷";
            }
            
            // 載入請款資料
            LoadPaymentData();
            
            // 載入已上傳檔案狀態
            LoadUploadedFileStatus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化請款核銷頁面時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入請款資料
    /// </summary>
    private void LoadPaymentData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 載入核定經費
            decimal approvedSubsidy = OFS_ClbPaymentHelper.GetApprovedSubsidy(ProjectID);
            
            // 載入請款記錄
            var paymentRecord = OFS_ClbPaymentHelper.GetPaymentRecord(ProjectID, 1);
            
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            
            // 判斷權限和狀態
            bool hasReviewPermission = currentUser != null && HasReviewPermission(currentUser);
            bool showReviewPanel = false;
            bool showSubmitButtons = false;
            
            string paymentStatus = "";
            decimal currentActualPaidAmount = 0;
            
            if (paymentRecord != null)
            {
                paymentStatus = paymentRecord.Status ?? "";
                currentActualPaidAmount = paymentRecord.CurrentActualPaidAmount ?? 0;
                
                // 審核視窗顯示條件：有審核權限 且 狀態為「審核中」
                showReviewPanel = hasReviewPermission && paymentStatus == "審核中";
                
                // 暫存與提送按鈕顯示條件：狀態為「暫存」或 null
                showSubmitButtons = (paymentStatus == "暫存" || string.IsNullOrEmpty(paymentStatus));
            }
            else
            {
                // 沒有記錄時可以暫存與提送
                showSubmitButtons = true;
            }
            
            // 將資料傳送到前端
            string jsCode = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    // 設定核定經費
                    const approvedSubsidyElement = document.getElementById('approvedSubsidy');
                    if (approvedSubsidyElement) {{
                        approvedSubsidyElement.textContent = '{approvedSubsidy:N0}';
                    }}
                    
                    // 設定全域變數供前端使用
                    window.paymentStatus = '{paymentStatus}';
                    window.currentActualPaidAmount = {currentActualPaidAmount};
                    
                    // 控制審核視窗顯示
                    const reviewPanel = document.querySelector('.scroll-bottom-panel');
                    if (reviewPanel) {{
                        reviewPanel.style.display = {(showReviewPanel ? "'block'" : "'none'")};
                    }}
                    
                    // 控制暫存與提送按鈕顯示  
                    const submitButtons = document.querySelector('.block-bottom');
                    if (submitButtons) {{
                        submitButtons.style.display = {(showSubmitButtons ? "'block'" : "'none'")};
                    }}
                    
                    // 控制實際撥款統計表格顯示（僅在審核通過時顯示）
                    const actualPaymentSection = document.getElementById('actualPaymentSection');
                    if (actualPaymentSection) {{
                        if ('{paymentStatus}' === '通過' && {currentActualPaidAmount} > 0) {{
                            actualPaymentSection.style.display = 'block';
                            
                            // 設定實際撥款金額 - CLB 僅一期，本期和累積都是同一個值
                            const currentActualPayment = document.getElementById('currentActualPayment');
                            const cumulativeActualPayment = document.getElementById('cumulativeActualPayment');
                            
                            if (currentActualPayment) {{
                                currentActualPayment.textContent = formatNumber({currentActualPaidAmount});
                            }}
                            if (cumulativeActualPayment) {{
                                cumulativeActualPayment.textContent = formatNumber({currentActualPaidAmount});
                            }}
                        }} else {{
                            actualPaymentSection.style.display = 'none';
                        }}
                    }}
                    
                    // 載入現有請款資料到表單
                    if ('{paymentStatus}' !== '') {{
                        loadExistingPaymentData();
                    }}
                    
                    // 控制累積實支金額欄位顯示方式
                    setupAccumulatedAmountField();
                    
                    // 初始化計算
                    if (typeof calculatePhase2Values === 'function') {{
                        calculatePhase2Values();
                    }}
                    
                    if (typeof updateRemainingAmount === 'function') {{
                        updateRemainingAmount();
                    }}
                    
                    // 執行檢視模式控制
                    if (typeof initializeViewMode === 'function') {{
                        initializeViewMode();
                    }}
                }});
                
                // 載入現有請款資料
                function loadExistingPaymentData() {{
                    {(paymentRecord != null ? $@"
                        const accumulatedAmountInput = document.getElementById('accumulatedAmountInput');
                        if (accumulatedAmountInput) {{
                            accumulatedAmountInput.value = {paymentRecord.TotalSpentAmount ?? 0};
                        }}
                        
                        const currentPaymentInput = document.getElementById('currentPayment');
                        if (currentPaymentInput) {{
                            currentPaymentInput.value = {paymentRecord.CurrentRequestAmount ?? 0};
                        }}" : "")}
                }}
                
                // 設定累積實支金額欄位顯示方式
                function setupAccumulatedAmountField() {{
                    const paymentStatus = window.paymentStatus || '';
                    const accumulatedAmountInput = document.getElementById('accumulatedAmountInput');
                    
                    if (accumulatedAmountInput && paymentStatus !== '暫存' && paymentStatus !== '') {{
                        // 設定為 disabled
                        accumulatedAmountInput.disabled = true;
                    }}
                }}
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "LoadPaymentData", jsCode, true);
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入請款資料時發生錯誤: {ex.Message}");
        }
    }
    
    
    
    /// <summary>
    /// 載入已上傳檔案狀態
    /// </summary>
    private void LoadUploadedFileStatus()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 檢查各種檔案類型的上傳狀態
            string[] fileCodes = { "PaymentIncomeStatement", "PaymentSubsidyList", "PaymentCostAllocation", "PaymentVouchers", "PaymentReceipts" };
            string[] fileTypes = { "1", "2", "3", "4", "5" };
            string[] fileNames = { "收支明細表", "受補助清單", "經費分攤表", "憑證", "領據（含帳戶資料）" };

            string jsCode = @"
                document.addEventListener('DOMContentLoaded', function() {
            ";

            for (int i = 0; i < fileCodes.Length; i++)
            {
                var uploadedFile = OFS_ClbPaymentHelper.GetUploadedFile(ProjectID, fileCodes[i]);
                if (uploadedFile != null)
                {
                    jsCode += $@"
                        // 設定檔案 {fileTypes[i]} 的狀態
                        document.getElementById('uploadStatus{fileTypes[i]}').textContent = '已上傳';
                        document.getElementById('uploadStatus{fileTypes[i]}').className = 'text-success';
                        document.getElementById('fileName{fileTypes[i]}').textContent = '{uploadedFile.FileName.Replace("'", "\\'")}';
                        document.getElementById('uploadedFile{fileTypes[i]}').style.display = 'flex';
                        document.getElementById('uploadedFile{fileTypes[i]}').dataset.fileId = '{uploadedFile.ID}';
                    ";
                }
            }

            jsCode += @"
                });
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "LoadUploadedFileStatus", jsCode, true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入已上傳檔案狀態時發生錯誤: {ex.Message}");
        }
    }

    #region WebMethods - 請款功能

    /// <summary>
    /// 提送請款申請 (暫存或提送)
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="isDraft">是否為暫存 (true=暫存, false=提送)</param>
    /// <param name="accumulatedAmount">累積實支金額</param>
    /// <param name="currentAmount">本期請款金額</param>
    /// <returns>操作結果</returns>
    [WebMethod]
    public static object SubmitReimbursement(string projectID, bool isDraft, decimal accumulatedAmount, decimal currentAmount)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫編號不能為空" };
            }

            // 基本驗證
            if (accumulatedAmount < 0)
            {
                return new { success = false, message = "累積實支金額不能為負數" };
            }

            if (currentAmount < 0)
            {
                return new { success = false, message = "本期請款金額不能為負數" };
            }

            // 呼叫 Helper 處理請款資料儲存
            var result = OFS_ClbPaymentHelper.SaveReimbursementData(projectID, isDraft, currentAmount, accumulatedAmount);
            
            if (result.success)
            {
                string actionText = isDraft ? "暫存" : "提送";
                return new { 
                    success = true, 
                    message = result.message, 
                    isDraft = isDraft 
                };
            }
            else
            {
                return new { 
                    success = false, 
                    message = result.message 
                };
            }
        }
        catch (Exception ex)
        {
            string actionText = isDraft ? "暫存" : "提送";
            return new { 
                success = false, 
                message = $"{actionText}失敗：{ex.Message}" 
            };
        }
    }

    /// <summary>
    /// 提交審查結果
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="reviewResult">審查結果 (pass/return)</param>
    /// <param name="reviewComment">審查意見</param>
    /// <param name="currentPayment">本期撥款金額</param>
    /// <returns>操作結果</returns>
    [WebMethod]
    public static object SubmitReview(string projectID, string reviewResult, string reviewComment, decimal currentPayment)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫編號不能為空" };
            }
            
            if (string.IsNullOrEmpty(reviewResult))
            {
                return new { success = false, message = "請選擇審查結果" };
            }

            // 取得當前使用者資訊
            var currentUser = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            if (currentUser == null)
            {
                return new { success = false, message = "無法取得使用者資訊" };
            }

            // 檢查審核權限
            if (!HasReviewPermission(currentUser))
            {
                return new { success = false, message = "您沒有權限進行審核" };
            }

            // 呼叫 Helper 處理審查結果提交
            var result = OFS_ClbPaymentHelper.SubmitReviewResult(projectID, reviewResult, reviewComment, currentPayment, currentUser.Account);
            
            if (result.success)
            {
                return new { 
                    success = true, 
                    message = result.message, 
                    reviewResult = reviewResult 
                };
            }
            else
            {
                return new { 
                    success = false, 
                    message = result.message 
                };
            }
        }
        catch (Exception ex)
        {
            return new { 
                success = false, 
                message = $"提交審查結果失敗：{ex.Message}" 
            };
        }
    }

    /// <summary>
    /// 檢查使用者是否具有審核權限
    /// </summary>
    /// <param name="currentUser">當前使用者資訊</param>
    /// <returns>是否具有審核權限</returns>
    private static bool HasReviewPermission(SessionHelper.UserInfoClass currentUser)
    {
        try
        {
            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }

            // 允許審核的角色：系統管理者、主管單位窗口、主管單位人員
            var allowedRoles = new[] { "系統管理者", "主管單位窗口", "主管單位人員" };

            foreach (string roleName in currentUser.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && allowedRoles.Contains(roleName))
                {
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 取得當前使用者資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
    private SessionHelper.UserInfoClass GetCurrentUserInfo()
    {
        try
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得使用者資訊時發生錯誤: {ex.Message}");
            return null;
        }
    }

    #endregion
}