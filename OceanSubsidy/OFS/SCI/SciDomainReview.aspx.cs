using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// 領域審查頁面
/// </summary>
public partial class OFS_SCI_SciDomainReview : System.Web.UI.Page
{
    private string ProjectID => Request["ProjectID"];
    private string Token => Request["Token"];
    private string ReviewID => hdnReviewID.Value;

    /// <summary>
    /// 執行單位名稱（用於風險評估連結）
    /// </summary>
    protected string OrgName { get; set; } = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 根據Request["ProjectID"] 得到ProjectID 並進行搜尋和初始化
            if (!string.IsNullOrEmpty(ProjectID))
            {
                LoadProjectData(ProjectID);
            }
            // 檢查Token是否已提交，如已提交則重導向
            if (!string.IsNullOrEmpty(Token) && OFS_SciDomainReviewHelper.IsReviewSubmitted(Token))
            {
                string script = @"
                    Swal.fire({
                        title: '此報告已審核',
                        text: '您已完成此報告的審核，無法再次進入。',
                        icon: 'info',
                        confirmButtonText: '確定'
                    }).then((result) => {
                        window.location.href = '../ReviewChecklist.aspx';
                    });
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "AlreadySubmitted", script, true);
                return;
            }

            // 根據Token取得審查記錄和評分資料
            if (!string.IsNullOrEmpty(Token))
            {
                LoadReviewDataByToken(Token);
            }
        }
    }

    private void LoadProjectData(string projectID)
    {
        // 使用 Helper 取得計畫資料
        DataRow projectData = OFS_SciDomainReviewHelper.GetProjectData(projectID);
        
        if (projectData != null)
        {
            string Status = "";
            switch (projectData["Status"].ToString())
            {
                case "領域審查":
                    Status = "領域審查";
                    break;
                case "技術審查":
                    Status = "技術審查";
                    break;
                case "21":
                    Status= "初審";
                    break;
                case "31":
                    Status= "複審";
                    break;
            }
            lblProjectNumber.Text = projectData["ProjectID"].ToString();
            lblYear.Text = projectData["Year"].ToString();
            lblProjectCategory.Text = projectData["ProjectCategory"].ToString(); // 從資料庫讀取補助案類別
            lblReviewGroup.Text = projectData["Field"].ToString();
            lblProjectName.Text = projectData["ProjectName"].ToString();
            lblApplicantUnit.Text = projectData["OrgName"].ToString();
            lblDocumentName.Text =  projectID + "_申請版_計畫書.pdf";
            lblReviewStatusName.Text =Status;

            // 檢查 PPT 檔案是否存在
            CheckPPTFileExists(projectID);
        }

        // 初始化風險評估等其他資料
        InitializeOtherData(projectID);
    }

    /// <summary>
    /// 檢查 PPT 檔案是否存在並設定按鈕顯示狀態
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void CheckPPTFileExists(string projectID)
    {
        try
        {
            // 判斷補助案類型
            string type = "";
            if (projectID.Contains("SCI"))
            {
                type = "SCI";
            }
            else if (projectID.Contains("CUL"))
            {
                type = "CUL";
            }
            else
            {
                // 不是 SCI 或 CUL，不顯示 PPT 按鈕
                btnDownloadPPT.Visible = false;
                return;
            }

            // 建立 PPT 檔案路徑
            string folderPath = Server.MapPath($"~/UploadFiles/OFS/{type}/{projectID}/TechReviewFiles");

            // 檢查資料夾是否存在
            if (!Directory.Exists(folderPath))
            {
                btnDownloadPPT.Visible = false;
                return;
            }

            // 搜尋 PPT 檔案 (*.ppt 或 *.pptx)
            var pptFiles = Directory.GetFiles(folderPath, "*.ppt*");

            if (pptFiles.Length > 0)
            {
                // 找到 PPT 檔案，顯示下載按鈕
                btnDownloadPPT.Visible = true;
            }
            else
            {
                // 沒有 PPT 檔案，不顯示按鈕
                btnDownloadPPT.Visible = false;
            }
        }
        catch (Exception)
        {
            // 發生錯誤時不顯示按鈕
            btnDownloadPPT.Visible = false;
        }
    }

    private void InitializeOtherData(string projectID)
    {
        // 取得計畫基本資料以獲得執行單位名稱
        DataRow projectData = OFS_SciDomainReviewHelper.GetProjectData(projectID);
        if (projectData == null)
        {
            return;
        }

        string orgName = projectData["OrgName"]?.ToString() ?? "";

        // 儲存到屬性供前端使用
        OrgName = orgName;

        // 使用 GetAuditRecordsByOrgName 取得風險評估記錄
        var auditRecords = AuditRecordsHelper.GetAuditRecordsByOrgName(orgName);

        // 統計筆數
        int recordCount = auditRecords != null ? auditRecords.Count : 0;
        lblRiskRecordCount.Text = recordCount.ToString();

        // 計算最高風險等級
        string riskLevel = "無";
        int maxRiskLevel = 0;

        if (auditRecords != null && auditRecords.Count > 0)
        {
            foreach (var record in auditRecords)
            {
                switch (record.Risk)
                {
                    case "Low":
                        if (maxRiskLevel < 1) maxRiskLevel = 1;
                        break;
                    case "Medium":
                        if (maxRiskLevel < 2) maxRiskLevel = 2;
                        break;
                    case "High":
                        if (maxRiskLevel < 3) maxRiskLevel = 3;
                        break;
                }
            }

            // 轉換風險等級顯示文字
            switch (maxRiskLevel)
            {
                case 1:
                    riskLevel = "低風險";
                    break;
                case 2:
                    riskLevel = "中風險";
                    break;
                case 3:
                    riskLevel = "高風險";
                    break;
                default:
                    riskLevel = "無";
                    break;
            }
        }

        lblRiskLevel.Text = riskLevel;
    }


    private void LoadReviewDataByToken(string token)
    {
        // 使用 Helper 根據 Token 取得審查記錄和評分資料（多筆）
        DataTable reviewDataTable = OFS_SciDomainReviewHelper.GetReviewDataByToken(token);
        
        if (reviewDataTable != null && reviewDataTable.Rows.Count > 0)
        {
            // 取第一筆的基本審查資訊（ReviewRecords 的資料）
            DataRow firstRow = reviewDataTable.Rows[0];
            
            // 取得 ReviewID 並存到 HiddenField
            if (firstRow["ReviewID"] != DBNull.Value)
            {
                hdnReviewID.Value = firstRow["ReviewID"].ToString();
            }
            
            // 設定審查委員資訊
            if (firstRow["ReviewerName"] != DBNull.Value)
            {
                lblReviewerName.Text = firstRow["ReviewerName"].ToString();
            }
            
            // 設定整體審查意見（ReviewComment）
            if (firstRow["ReviewComment"] != DBNull.Value)
            {
                txtReviewComment.Text = firstRow["ReviewComment"].ToString();
            }
            
            
            // 更新最後修改時間
            if (firstRow["CreateTime"] != DBNull.Value)
            {
                DateTime createTime = Convert.ToDateTime(firstRow["CreateTime"]);
                lblLastUpdateTime.Text = createTime.ToString("yyyy/MM/dd HH:mm:ss");
                lblLastUpdateBy.Text = lblReviewerName.Text;
            }
            
            // 綁定評審項目到 Repeater
            BindReviewItems(reviewDataTable);
        }
    }

    private void BindReviewItems(DataTable reviewDataTable)
    {
        // 建立評審項目資料表
        DataTable itemsTable = new DataTable();
        itemsTable.Columns.Add("Id", typeof(string));
        itemsTable.Columns.Add("ItemName", typeof(string));
        itemsTable.Columns.Add("Weight", typeof(string));
        itemsTable.Columns.Add("Score", typeof(string));

        // 將資料加入表格
        foreach (DataRow row in reviewDataTable.Rows)
        {
            if (row["ItemName"] != DBNull.Value)
            {
                DataRow newRow = itemsTable.NewRow();
                newRow["Id"] = row["Id"] ?? "";
                newRow["ItemName"] = row["ItemName"] ?? "";
                newRow["Weight"] = row["Weight"] ?? "";
                newRow["Score"] = row["Score"] ?? "";
                itemsTable.Rows.Add(newRow);
            }
        }

        // 綁定到 Repeater
        rptReviewItems.DataSource = itemsTable;
        rptReviewItems.DataBind();
    }

    /// <summary>
    /// 從 Repeater 中取得各評審項目的評分
    /// </summary>
    /// <returns>項目ID和評分的字典</returns>
    private Dictionary<string, decimal> GetItemScoresFromRepeater()
    {
        var itemScores = new Dictionary<string, decimal>();

        foreach (RepeaterItem item in rptReviewItems.Items)
        {
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                var hdnItemId = item.FindControl("hdnItemId") as HiddenField;
                var txtItemScore = item.FindControl("txtItemScore") as TextBox;

                if (hdnItemId != null && txtItemScore != null)
                {
                    string itemId = hdnItemId.Value;
                    string scoreText = txtItemScore.Text.Trim();

                    if (!string.IsNullOrEmpty(itemId) && !string.IsNullOrEmpty(scoreText))
                    {
                        if (decimal.TryParse(scoreText, out decimal score))
                        {
                            // 驗證評分範圍 (0-100)
                            if (score >= 0 && score <= 100)
                            {
                                itemScores[itemId] = score;
                            }
                        }
                    }
                }
            }
        }

        return itemScores;
    }

    protected void btnDownloadDocument_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                ShowMessage("無法取得計畫編號", false);
                return;
            }

            string downloadUrl = "";

            // 判斷補助案類型並使用對應的下載服務
            if (ProjectID.Contains("CUL"))
            {
                // CUL 使用 DownloadPdf.ashx 下載評審版
                downloadUrl = $"/Service/OFS/DownloadPdf.ashx?ProjectID={ProjectID}";
            }
            else if (ProjectID.Contains("SCI"))
            {
                // SCI 使用 SCI_Download.ashx 的 DownloadPlan
                downloadUrl = $"/Service/SCI_Download.ashx?action=downloadPlan&projectID={ProjectID}";
            }
            else
            {
                ShowMessage("無法識別補助案類型", false);
                return;
            }

            // 重導向至下載服務
            Response.Redirect(downloadUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            ShowMessage($"下載檔案時發生錯誤：{ex.Message}", false);
        }
    }

    protected void btnDownloadPPT_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                ShowMessage("無法取得計畫編號", false);
                return;
            }

            // 判斷補助案類型
            string type = "";
            if (ProjectID.Contains("SCI"))
            {
                type = "SCI";
            }
            else if (ProjectID.Contains("CUL"))
            {
                type = "CUL";
            }
            else
            {
                ShowMessage("無法識別補助案類型", false);
                return;
            }

            // 建立 PPT 檔案路徑
            string folderPath = Server.MapPath($"~/UploadFiles/OFS/{type}/{ProjectID}/TechReviewFiles");

            // 檢查資料夾是否存在
            if (!Directory.Exists(folderPath))
            {
                ShowMessage("找不到 PPT 檔案", false);
                return;
            }

            // 搜尋 PPT 檔案 (*.ppt 或 *.pptx)，取最新的檔案
            var pptFiles = Directory.GetFiles(folderPath, "*.ppt*")
                                   .OrderByDescending(f => File.GetLastWriteTime(f))
                                   .ToArray();

            if (pptFiles.Length == 0)
            {
                ShowMessage("找不到 PPT 檔案", false);
                return;
            }

            // 取得最新的 PPT 檔案
            string pptFilePath = pptFiles[0];
            string fileName = Path.GetFileName(pptFilePath);

            // 設定回應標頭並下載檔案
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            Response.TransmitFile(pptFilePath);
            Response.Flush();
            Response.End();
        }
        catch (Exception ex)
        {
            ShowMessage($"下載 PPT 檔案時發生錯誤：{ex.Message}", false);
        }
    }

    protected void btnSaveDraft_Click(object sender, EventArgs e)
    {
        try
        {
            // 驗證 ReviewID
            if (string.IsNullOrEmpty(ReviewID))
            {
                ShowMessage("錯誤：無法取得審查記錄ID", false);
                return;
            }

            // 取得各項目評分
            var itemScores = GetItemScoresFromRepeater();
            
            // 取得審查意見
            string reviewComment = txtReviewComment.Text.Trim();

            // 呼叫 Helper 進行暫存 (IsSubmit = false)
            bool success = OFS_SciDomainReviewHelper.UpdateReviewScores(ReviewID, itemScores, reviewComment, false);

            if (success)
            {
                ShowMessage("暫存成功", true);
                // 更新最後修改時間顯示
                lblLastUpdateTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                lblLastUpdateBy.Text = lblReviewerName.Text;
            }
            else
            {
                ShowMessage("暫存失敗", false);
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"暫存時發生錯誤：{ex.Message}", false);
        }
    }

    protected void btnSubmitReview_Click(object sender, EventArgs e)
    {
        try
        {
            // 驗證 ReviewID
            if (string.IsNullOrEmpty(ReviewID))
            {
                ShowMessage("錯誤：無法取得審查記錄ID", false);
                return;
            }

            // 取得各項目評分
            var itemScores = GetItemScoresFromRepeater();

            // 驗證是否所有項目都已評分
            if (itemScores.Count == 0)
            {
                ShowMessage("請至少為一個項目評分", false);
                return;
            }

            // 取得審查意見
            string reviewComment = txtReviewComment.Text.Trim();
            if (string.IsNullOrEmpty(reviewComment))
            {
                ShowMessage("請輸入審查意見", false);
                return;
            }

            // 呼叫 Helper 進行提送 (IsSubmit = true)
            bool success = OFS_SciDomainReviewHelper.UpdateReviewScores(ReviewID, itemScores, reviewComment, true);

            if (success)
            {
                ShowMessage("審查結果提送成功", true);
                // 更新最後修改時間顯示
                lblLastUpdateTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                lblLastUpdateBy.Text = lblReviewerName.Text;
                
                // 提送後可以考慮禁用按鈕或跳轉頁面
                btnSubmitReview.Enabled = false;
                btnSaveDraft.Enabled = false;
                string script = "window.location.href = '../ReviewChecklist.aspx';";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "Redirect", script, true);
                
            }
            else
            {
                ShowMessage("提送失敗", false);
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"提送時發生錯誤：{ex.Message}", false);
        }
    }

    /// <summary>
    /// 顯示訊息的輔助方法
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="isSuccess">是否為成功訊息</param>
    private void ShowMessage(string message, bool isSuccess)
    {
        // 可以使用 JavaScript Alert 或其他方式顯示訊息
        string script = $"alert('{message}');";
        ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }
}