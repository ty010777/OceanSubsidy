using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.App;

public partial class OFS_CLB_ClbApproved : System.Web.UI.Page
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
        else
        {
            // PostBack 時重新設定審核者資訊
            SetReviewerInfoFromDatabase();
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
                master.ProjectInfoText = $"{ProjectID}/核定計畫";
            }
            
            // 載入計畫資料
            LoadProjectData();
            
            // 設定審核者資訊（從資料庫讀取）
            SetReviewerInfoFromDatabase();
            
            // 載入移轉案件的部門下拉選單
            LoadDepartmentDropDown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化核定計畫頁面時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入計畫資料
    /// </summary>
    private void LoadProjectData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 設定 UserControl 的 ProjectID
            ucClbApplication.ProjectID = ProjectID;

            // 從資料庫檢查計畫變更狀態
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            bool canEdit = false;

            if (projectMain != null)
            {
                // 只有當 IsProjChanged = 1 (計畫變更中) 時才可以編輯
                canEdit = projectMain.IsProjChanged == 1;
            }

            // 設定 UserControl 的編輯權限
            ucClbApplication.IsReadOnly = !canEdit;


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入計畫資料時發生錯誤: {ex.Message}");
        }
    }
    
    #region 移轉案件功能
    
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
    /// 移轉案件 WebMethod (AJAX 呼叫)
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object TransferProject(string ProjectID, string DepartmentID, string DepartmentName, string ReviewerAccount, string ReviewerName)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(DepartmentID))
            {
                return new { success = false, message = "請選擇部門" };
            }

            if (string.IsNullOrEmpty(ReviewerAccount))
            {
                return new { success = false, message = "請選擇承辦人員" };
            }

            if (string.IsNullOrEmpty(ProjectID))
            {
                return new { success = false, message = "找不到計畫ID" };
            }

            // 取得案件資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                return new { success = false, message = "找不到案件資料" };
            }

            // 更新承辦人相關的三個欄位
            OFS_ClbApplicationHelper.UpdateProjectSupervisoryInfo(
                ProjectID,
                ReviewerAccount,
                ReviewerName,
                DepartmentName
            );

            return new { success = true, message = "案件移轉完成" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"移轉案件時發生錯誤: {ex.Message}");
            return new { success = false, message = "移轉案件時發生錯誤，請稍後再試" };
        }
    }

    /// <summary>
    /// 處理計畫變更申請 WebMethod (AJAX 呼叫)
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object ProcessPlanChange(string ProjectID, string ChangeReason)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(ProjectID))
            {
                return new { success = false, message = "找不到計畫ID" };
            }

            if (string.IsNullOrEmpty(ChangeReason))
            {
                return new { success = false, message = "請輸入計畫變更原因" };
            }

            // 取得案件資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                return new { success = false, message = "找不到案件資料" };
            }

            // 更新 IsProjChanged 為 1 (計畫變更中)
            OFS_ClbApplicationHelper.UpdateProjectChangeStatus(ProjectID, 1);

            return new { success = true, message = "計畫變更申請已通過" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"處理計畫變更申請時發生錯誤: {ex.Message}");
            return new { success = false, message = "處理計畫變更申請時發生錯誤，請稍後再試" };
        }
    }
    
    #endregion
    
    #region 私有方法
    
    /// <summary>
    /// 載入移轉案件的部門下拉選單
    /// </summary>
    private void LoadDepartmentDropDown()
    {
        try
        {
            // 清空現有項目
            ddlDepartment.Items.Clear();
            
            // 加入預設選項
            ddlDepartment.Items.Add(new ListItem("請選擇部門", ""));
            // 載入指定的審核單位（用於移轉案件）
            var departmentData = SysUnitHelper.QueryReviewUnits();
            
            if (departmentData != null && departmentData.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in departmentData.Rows)
                {
                    string unitID = row["UnitID"].ToString();
                    string unitName = row["UnitName"].ToString();
                    ddlDepartment.Items.Add(new ListItem(unitName, unitID));
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入部門下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入承辦人員下拉選單
    /// </summary>
    /// <param name="unitID">部門ID</param>
    private void LoadReviewerDropDown(string unitID)
    {
        try
        {
            // 清空現有項目
            ddlReviewer.Items.Clear();
            
            // 加入預設選項
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
            
            if (string.IsNullOrEmpty(unitID))
            {
                return;
            }
            
            // 從資料庫載入承辦人員資料
            var reviewerData = SysUserHelper.QueryReviewersByUnitID(unitID);
            
            if (reviewerData != null && reviewerData.Rows.Count > 0)
            {
                for (int i = 0; i < reviewerData.Rows.Count; i++)
                {
                    string account = reviewerData.Rows[i]["Account"].ToString();
                    string name = reviewerData.Rows[i]["Name"].ToString();
                    string displayText = $"{name} ({account})";
                    
                    ddlReviewer.Items.Add(new ListItem(displayText, account));
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員下拉選單時發生錯誤");
        }
    }
    
    /// <summary>
    /// 從資料庫設定審核者資訊
    /// </summary>
    private void SetReviewerInfoFromDatabase()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 從 OFS_CLB_Project_Main 表中讀取承辦人員資訊
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            
            if (projectMain != null && !string.IsNullOrEmpty(projectMain.SupervisoryPersonName))
            {
                // 更新頁面顯示的承辦人員名稱
                lblReviewerName.Text = projectMain.SupervisoryPersonName;
            }
            else
            {
                lblReviewerName.Text = "未分配承辦人員";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定審核者資訊時發生錯誤");
        }
    }
    
 
    
    /// <summary>
    /// 顯示 SweetAlert 訊息
    /// </summary>
    private void ShowSweetAlert(string title, string text, string icon)
    {
        string script = $@"
            Swal.fire({{
                title: '{title}',
                text: '{text}',
                icon: '{icon}',
                confirmButtonText: '確定'
            }});
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSweetAlert", script, true);
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