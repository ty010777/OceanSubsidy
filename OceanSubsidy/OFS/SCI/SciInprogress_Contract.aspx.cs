using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;

public partial class OFS_SCI_SciInprogress_Contract : System.Web.UI.Page
{
    /// <summary>
    /// 目前處理的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否有計畫ID
            if (string.IsNullOrEmpty(ProjectID))
            {
                Response.Redirect("~/OFS/inprogressList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 初始化頁面
                InitializePage();
            }
        }
        catch (Exception ex)
        {
            // 例外處理
            System.Diagnostics.Debug.WriteLine($"頁面載入時發生錯誤: {ex.Message}");
        }
    }

    private void InitializePage()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 設定計畫基本資料
            lblProjectID.Text = ProjectID;
            // 從 OFS_SCI_Application_Main 取得計畫名稱
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            lblProjectName.Text = applicationMain.ProjectNameTw;
            lblExecutingUnit.Text = applicationMain.OrgName;
            // 執行期程 (使用民國年格式)
            string startDateStr = applicationMain.StartTime.HasValue ? DateTimeHelper.ToMinguoDate(applicationMain.StartTime.Value) : "";
            string endDateStr = applicationMain.EndTime.HasValue ? DateTimeHelper.ToMinguoDate(applicationMain.EndTime.Value) : "";
            
            if (!string.IsNullOrEmpty(startDateStr) && !string.IsNullOrEmpty(endDateStr))
            {
                lblExecutionPeriod.Text = $"{startDateStr} ～ {endDateStr}";
            }
            else
            {
                lblExecutionPeriod.Text = "未設定執行期程";
            }

            // 載入契約資料
            LoadContractData();


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化頁面時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 載入契約資料
    /// </summary>
    private void LoadContractData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
                return;

            // 從 OFS_SCI_Project_Main 取得契約資料
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (projectMain != null)
            {
                // 載入發文文號
                txtDocumentNumber.Text = projectMain.PubNumber ?? "";
                
                // 載入簽約日期
                if (projectMain.ContractDate.HasValue)
                {
                    txtContractDate.Text = projectMain.ContractDate.Value.ToString("yyyy-MM-dd");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入契約資料時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 儲存契約資料
    /// </summary>
    /// <param name="isSubmit">是否為提送</param>
    /// <returns>是否儲存成功</returns>
    private bool SaveContractData(bool isSubmit = false)
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
                return false;

            // 驗證必填欄位
            if (isSubmit)
            {
                if (string.IsNullOrWhiteSpace(txtDocumentNumber.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                        "alert('請填寫發文文號');", true);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtContractDate.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                        "alert('請選擇簽約日期');", true);
                    return false;
                }
            }

            // 取得現有專案資料
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (projectMain == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    "alert('找不到專案資料');", true);
                return false;
            }

            // 處理簽約日期
            DateTime? contractDate = null;
            if (!string.IsNullOrWhiteSpace(txtContractDate.Text))
            {
                if (DateTime.TryParse(txtContractDate.Text, out DateTime parsedDate))
                {
                    contractDate = parsedDate;
                }
            }

            // 儲存到資料庫
            OFS_SciApplicationHelper.updateContractData(ProjectID, txtDocumentNumber.Text.Trim(), contractDate);
            if (isSubmit)
            {
                InprogressListHelper.UpdateLastOperation(ProjectID, "已完成契約資料");
                InprogressListHelper.UpdateTaskCompleted(ProjectID,"Contract", true);
            }
            // 顯示成功訊息
            string message = isSubmit ? "契約資料提送成功" : "契約資料暫存成功";
            ScriptManager.RegisterStartupScript(this, GetType(), "success", 
                $"alert('{message}');", true);

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"儲存契約資料時發生錯誤: {ex.Message}");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                $"alert('儲存失敗：{ex.Message}');", true);
            return false;
        }
    }

    // /// <summary>
    // /// 暫存按鈕點擊事件
    // /// </summary>
    // protected void btnSave_Click(object sender, EventArgs e)
    // {
    //     SaveContractData(false);
    // }

    /// <summary>
    /// 提送按鈕點擊事件
    /// </summary>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        SaveContractData(true);
    }
   
}