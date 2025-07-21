using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// 科專計畫審核系統專用 Master Page
/// 提供統一的審核介面框架和進度條導航
/// </summary>
public partial class OFS_SCI_OFSReviewMaster : System.Web.UI.MasterPage
{
    #region 屬性

    /// <summary>
    /// 目前審核的計畫ID
    /// </summary>
    public string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                // 檢查計畫ID
                if (string.IsNullOrEmpty(ProjectID))
                {
                    Response.Redirect("~/OFS/ApplicationChecklist.aspx");
                    return;
                }

                // 初始化頁面
                InitializeMasterPage();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "Master Page 載入時發生錯誤");
        }
    }

    #endregion

    #region 公用方法

    /// <summary>
    /// 設定當前活躍的步驟
    /// </summary>
    /// <param name="stepNumber">步驟編號 (1-5)</param>
    public void SetActiveStep(int stepNumber)
    {
        try
        {
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    // 設定當前活躍步驟
                    const stepItems = document.querySelectorAll('.application-step .step-item');
                    stepItems.forEach(item => {{
                        item.classList.remove('active');
                        const statusElement = item.querySelector('.step-status');
                        if (statusElement) {{
                            statusElement.textContent = '';
                        }}
                    }});
                    
                    const activeStep = document.querySelector('[data-review-step=""{stepNumber}""]');
                    if (activeStep) {{
                        activeStep.classList.add('active');
                        let statusElement = activeStep.querySelector('.step-status');
                        if (!statusElement) {{
                            statusElement = document.createElement('div');
                            statusElement.className = 'step-status';
                            activeStep.querySelector('.step-content').appendChild(statusElement);
                        }}
                        statusElement.textContent = '檢視中';
                    }}
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetActiveStep", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定活躍步驟時發生錯誤");
        }
    }

    /// <summary>
    /// 設定步驟狀態
    /// </summary>
    /// <param name="stepNumber">步驟編號</param>
    /// <param name="status">狀態文字 (如：檢視中、已完成等)</param>
    /// <param name="isActive">是否為當前活躍步驟</param>
    public void SetStepStatus(int stepNumber, string status, bool isActive = false)
    {
        try
        {
            string activeClass = isActive ? "active" : "";
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    const step = document.querySelector('[data-review-step=""{stepNumber}""]');
                    if (step) {{
                        if ('{isActive}' === 'True') {{
                            step.classList.add('active');
                        }} else {{
                            step.classList.remove('active');
                        }}
                        
                        let statusElement = step.querySelector('.step-status');
                        if (!statusElement) {{
                            statusElement = document.createElement('div');
                            statusElement.className = 'step-status';
                            step.querySelector('.step-content').appendChild(statusElement);
                        }}
                        statusElement.textContent = '{status}';
                    }}
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), $"SetStepStatus_{stepNumber}", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定步驟狀態時發生錯誤");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 初始化 Master Page
    /// </summary>
    private void InitializeMasterPage()
    {
        try
        {
            // 可以在這裡加入任何需要在 Master Page 載入時執行的邏輯
            // 例如：檢查使用者權限、載入通用資料等
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化 Master Page 時發生錯誤");
        }
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    /// <param name="ex">例外物件</param>
    /// <param name="context">發生錯誤的上下文</param>
    private void HandleException(Exception ex, string context)
    {
        // 記錄錯誤
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        
        // 可以在這裡加入更多的錯誤處理邏輯
        // 例如：記錄到日誌檔案、發送錯誤通知等
    }

    #endregion
}