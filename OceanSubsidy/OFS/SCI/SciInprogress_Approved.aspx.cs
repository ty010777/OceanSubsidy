using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;

public partial class OFS_SCI_SciInprogress_Approved : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 目前檢視的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    #region 頁面事件

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
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }

    #endregion

    #region 按鈕事件

    /// <summary>
    /// 部門下拉選單變更事件
    /// </summary>
    protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string selectedUnitID = ddlDepartment.SelectedValue;
            LoadReviewerDropDown(selectedUnitID);
            
            // 更新 UpdatePanel
            upTransferCase.Update();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員時發生錯誤");
        }
    }

    /// <summary>
    /// 確認移轉案件
    /// </summary>
    protected void btnConfirmTransfer_Click(object sender, EventArgs e)
    {
        try
        {
            string selectedDepartmentID = ddlDepartment.SelectedValue;
            string selectedReviewerAccount = ddlReviewer.SelectedValue;
            
            // 驗證必填欄位
            if (string.IsNullOrEmpty(selectedDepartmentID))
            {
                ShowSweetAlert("錯誤", "請選擇部門", "error");
                return;
            }
            
            if (string.IsNullOrEmpty(selectedReviewerAccount))
            {
                ShowSweetAlert("錯誤", "請選擇承辦人員", "error");
                return;
            }
            
            if (string.IsNullOrEmpty(ProjectID))
            {
                ShowSweetAlert("錯誤", "找不到計畫ID", "error");
                return;
            }
            
            // 取得選中的承辦人員詳細資訊
            var reviewerData = SysUserHelper.QueryUserByAccount(selectedReviewerAccount);
            if (reviewerData == null || reviewerData.Rows.Count == 0)
            {
                ShowSweetAlert("錯誤", "找不到選擇的承辦人員資訊", "error");
                return;
            }
            
            // 取得部門名稱
            var departmentData = SysUnitHelper.QueryByID(selectedDepartmentID);
            if (departmentData == null || departmentData.Rows.Count == 0)
            {
                ShowSweetAlert("錯誤", "找不到選擇的部門資訊", "error");
                return;
            }
            
            string reviewerName = reviewerData.Rows[0]["Name"].ToString();
            string departmentName = departmentData.Rows[0]["UnitName"].ToString();
            
            // 建立更新物件
            var projectMain = new OFS_SCI_Project_Main
            {
                ProjectID = ProjectID,
                SupervisoryPersonAccount = selectedReviewerAccount,
                SupervisoryPersonName = reviewerName,
                SupervisoryUnit = departmentName,
                updated_at = DateTime.Now
            };
            
            // 更新資料庫
            OFS_SciApplicationHelper.UpdateOFS_SCIVersion(projectMain);
            
            // 更新頁面上的承辦人員資訊顯示
            LoadCurrentReviewerInfo();
            
            ShowSweetAlert("成功", "案件移轉完成", "success");
            
            // 清空選項並關閉 Modal
            ddlDepartment.SelectedIndex = 0;
            ddlReviewer.Items.Clear();
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
            
            // 關閉 Modal
            string script = @"
                document.getElementById('transferCaseModal').querySelector('.btn-close').click();
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "closeModal", script, true);
            LoadAllUserControlData();
        }
        catch (Exception ex)
        {
            HandleException(ex, "移轉案件時發生錯誤");
            ShowSweetAlert("錯誤", "移轉案件時發生錯誤，請稍後再試", "error");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 載入所有申請資料到 UserControl (檢視模式)
            LoadAllUserControlData();

            // 設定計畫資訊到 Master Page
            SetProjectInfoToMaster();
            
            // 載入移轉案件的部門下拉選單
            LoadDepartmentDropDown();
            
            // 載入目前的承辦人員資訊
            LoadCurrentReviewerInfo();
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
        }
    }

    /// <summary>
    /// 載入所有 UserControl 資料
    /// </summary>
    private void LoadAllUserControlData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 第一頁：申請表/聲明書 (檢視模式)
            ucSciApplication.LoadData(ProjectID);
            
            // 第二頁：期程／工作項目／查核 (檢視模式)
            ucSciWorkSch.LoadData(ProjectID);
            
            // 第三頁：經費／人事 (檢視模式)
            ucSciFunding.LoadData(ProjectID);
            
            // 第四頁：其他 (檢視模式)
            ucSciRecusedList.LoadData(ProjectID);
            
            // 第五頁：上傳附件/提送申請 (檢視模式)
            ucSciUploadAttachments.LoadData(ProjectID);
            
            // 註冊 JavaScript，在所有資料載入完成後重新應用檢視模式
            RegisterViewModeScript();
            
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入 UserControl 資料時發生錯誤");
        }
    }

    /// <summary>
    /// 設定計畫資訊到 Master Page
    /// </summary>
    private void SetProjectInfoToMaster()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
        
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定計畫資訊時發生錯誤");
        }
    }

    /// <summary>
    /// 註冊檢視模式 JavaScript，在所有動態內容載入後執行
    /// </summary>
    private void RegisterViewModeScript()
    {
        string script = @"
           document.addEventListener('DOMContentLoaded', function() {
                document.body.classList.add('app-mode-view');

                // 禁用所有表單元素，但跳過 UserInfo div 內的元素
                var formElements = document.querySelectorAll('input, textarea, select');
                formElements.forEach(function(element) {
                    // 檢查元素是否在 UserInfo div 內
                    if (!element.closest('#UserInfo, #FuncBtn,#transferCaseModal')) {
                        element.disabled = true;
                    }
                });

                // 將所有有 view-mode class 的元件加上 d-none class
                var viewModeElements = document.querySelectorAll('.view-mode');
                viewModeElements.forEach(function(element) {
                    element.classList.add('d-none');
                });

                // 特別處理一些可能動態生成的元素
                setTimeout(function() {
                    var dynamicElements = document.querySelectorAll('input ,textarea, select');
                    dynamicElements.forEach(function(element) {
                        // 檢查元素是否在 UserInfo div 內且尚未被禁用
                        if (!element.disabled && !element.closest('#UserInfo, #FuncBtn,#transferCaseModal')) {
                            element.disabled = true;
                        }
                    });

                    // 再次處理可能動態生成的 view-mode 元素
                    var dynamicViewModeElements = document.querySelectorAll('.view-mode');
                    dynamicViewModeElements.forEach(function(element) {
                        if (!element.classList.contains('d-none')) {
                            element.classList.add('d-none');
                        }
                    });
                }, 1000);
            });
        ";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ApplyViewMode", script, true);
    }

    /// <summary>
    /// 載入移轉案件的部門下拉選單
    /// </summary>
    private void LoadDepartmentDropDown()
    {
        try
        {
            // 載入指定的審核單位（用於移轉案件）
            var departmentData = SysUnitHelper.QueryReviewUnits();
            
            // 清空並重新綁定部門下拉選單
            ddlDepartment.Items.Clear();
            ddlDepartment.Items.Add(new ListItem("請選擇部門", ""));
            
            if (departmentData != null && departmentData.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in departmentData.Rows)
                {
                    string unitID = row["UnitID"].ToString();
                    string unitName = row["UnitName"].ToString();
                    ddlDepartment.Items.Add(new ListItem(unitName, unitID));
                }
            }
            
            // 初始化承辦人員下拉選單
            ddlReviewer.Items.Clear();
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入部門下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入指定部門的承辦人員下拉選單
    /// </summary>
    private void LoadReviewerDropDown(string unitID)
    {
        try
        {
            // 清空承辦人員下拉選單
            ddlReviewer.Items.Clear();
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
            
            if (!string.IsNullOrEmpty(unitID))
            {
                // 根據部門ID載入使用者
                var userData = SysUserHelper.QueryUserByUnitID(unitID);
                
                if (userData != null && userData.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in userData.Rows)
                    {
                        string account = row["Account"].ToString();
                        string name = row["Name"].ToString();
                        ddlReviewer.Items.Add(new ListItem(name, account));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入目前的承辦人員資訊
    /// </summary>
    private void LoadCurrentReviewerInfo()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 從 OFS_SCI_Project_Main 表中讀取承辦人員資訊（參考 SciApplicationReview.aspx.cs）
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            
            if (projectMain != null)
            {
                // 設定承辦人員顯示文字
                if (!string.IsNullOrEmpty(projectMain.SupervisoryPersonName))
                {
                    lblCurrentReviewer.Text = projectMain.SupervisoryPersonName;
                }
                else
                {
                    lblCurrentReviewer.Text = "未設定";
                }
            }
            else
            {
                lblCurrentReviewer.Text = "未設定";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員資訊時發生錯誤");
            lblCurrentReviewer.Text = "載入錯誤";
        }
    }

    /// <summary>
    /// 顯示 SweetAlert 提示訊息
    /// </summary>
    private void ShowSweetAlert(string title, string text, string icon)
    {
        try
        {
            string script = $@"
                Swal.fire({{
                    title: '{title}',
                    text: '{text}',
                    icon: '{icon}',
                    confirmButtonText: '確定'
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "顯示提示訊息時發生錯誤");
        }
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
    }

    #endregion
}