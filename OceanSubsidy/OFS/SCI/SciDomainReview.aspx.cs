using System;
using System.Collections.Generic;
using System.Data;
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
            lblProjectNumber.Text = projectData["ProjectID"].ToString();
            lblYear.Text = projectData["Year"].ToString();
            lblProjectCategory.Text = projectData["ProjectCategory"].ToString(); // 從資料庫讀取補助案類別
            lblReviewGroup.Text = projectData["Field"].ToString();
            lblProjectName.Text = projectData["ProjectName"].ToString();
            lblApplicantUnit.Text = projectData["OrgName"].ToString();
            lblDocumentName.Text =  projectID + "_送審版_計畫書.pdf";
        }

        // 初始化風險評估等其他資料
        InitializeOtherData(projectID);
    }

    private void InitializeOtherData(string projectID)
    {
        // 取得風險評估資料（寫死資料）
        string riskLevel = OFS_SciDomainReviewHelper.GetRiskLevel(projectID);
        int recordCount = OFS_SciDomainReviewHelper.GetRiskRecordCount(projectID);
        
        lblRiskLevel.Text = riskLevel;
        lblRiskRecordCount.Text = recordCount.ToString();
        lblModalRiskLevel.Text = riskLevel;
        
        // Modal 資料初始化
        lblExecutingUnit.Text = "海洋委員會科技文教處科技科";
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
                // CUL 使用 DownloadPdf.ashx
                downloadUrl = $"/Service/OFS/DownloadPdf.ashx?Type=CUL&ProjectID={ProjectID}&Version=1";
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