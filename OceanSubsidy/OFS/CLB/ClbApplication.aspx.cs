using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OFS_CLB_ClbApplication : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 初始化頁面
            InitializePage();
        }
    }

    private void InitializePage()
    {
        // 檢查 URL 參數是否有 ProjectID
        string projectID = Request.QueryString["ProjectID"];
        
        // 將 ProjectID 傳遞給 UserControl，讓它自己處理載入邏輯
        ucClbApplication.ProjectID = projectID ?? "";
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Button clickedButton = (Button)sender;
        
        try
        {
            if (clickedButton.ID == "btnTempSave")
            {
                // 暫存邏輯
                TempSave();
            }
            else if (clickedButton.ID == "btnSubmit")
            {
                // 儲存並進入下一步
                SaveAndNext();
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            Response.Write($"<script>alert('操作失敗：{ex.Message}');</script>");
        }
    }

    private void TempSave()
    {
        try
        {
            // 直接引用 UserControl
            // 執行暫存（傳入 true 表示暫存）
            string projectID = ucClbApplication.SaveBasicData(true);
            
            if (!string.IsNullOrEmpty(projectID))
            {
                // 顯示成功訊息，包含 ProjectID，按下確定後導向
                string script = $@"
                    Swal.fire({{
                        icon: 'success',
                        title: '暫存成功！',
                        html: '計畫編號：<strong>{projectID}</strong><br>資料已成功暫存',
                        confirmButtonText: '確定'
                    }}).then((result) => {{
                        if (result.isConfirmed) {{
                            // 導向有 ProjectID 參數的頁面
                            window.location.href = 'ClbApplication.aspx?ProjectID={projectID}';
                        }}
                    }});";
                
                ClientScript.RegisterStartupScript(this.GetType(), "TempSaveSuccess", script, true);
            }
            else
            {
                throw new Exception("儲存失敗，請檢查必填欄位");
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: '暫存失敗',
                    text: '{ex.Message}',
                    confirmButtonText: '確定'
                }});";
            
            ClientScript.RegisterStartupScript(this.GetType(), "TempSaveError", script, true);
        }
    }

    private void SaveAndNext()
    {
        try
        {
            // 直接引用 UserControl
            // 執行正式儲存（傳入 false 表示正式儲存）
            string projectID = ucClbApplication.SaveBasicData(false);
            
            if (!string.IsNullOrEmpty(projectID))
            {
                // 儲存成功，切換到上傳附件步驟
                string script = $@"
                    Swal.fire({{
                        icon: 'success',
                        title: '儲存成功！',
                        html: '計畫編號：<strong>{projectID}</strong><br>即將進入上傳附件步驟',
                        timer: 2000,
                        showConfirmButton: false
                    }}).then(() => {{
                        // 切換到上傳附件步驟
                        switchToStep(1);
                    }});";
                
                ClientScript.RegisterStartupScript(this.GetType(), "SaveAndNextSuccess", script, true);
            }
            else
            {
                throw new Exception("儲存失敗，請檢查必填欄位");
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: '儲存失敗',
                    text: '{ex.Message}',
                    confirmButtonText: '確定'
                }});";
            
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAndNextError", script, true);
        }
    }
}