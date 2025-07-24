using System;
[Serializable]
public class ReviewChecklistItem
{
    public string ProjectID { get; set; }
    public string Statuses { get; set; }
    public string StatusesName { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal SeqPoint { get; set; }
    public string SupervisoryUnit { get; set; }
    public string SupervisoryPersonName { get; set; }
    public string SupervisoryPersonAccount { get; set; }
    public string UserAccount { get; set; }
    public string UserOrg { get; set; }
    public string UserName { get; set; }
    public string Form1Status { get; set; }
    public string Form2Status { get; set; }
    public string Form3Status { get; set; }
    public string Form4Status { get; set; }
    public string Form5Status { get; set; }
    public string CurrentStep { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    
    // Additional fields for display
    public string ProjectNameTw { get; set; }
    public string OrgName { get; set; }
    public string Year { get; set; }
    public string SubsidyPlanType { get; set; }
    public string ApplicationAmount { get; set; }
    public bool? isWithdrawal { get; set; }
    public bool? isExists { get; set; }
    
    // Progress fields
    public string ReviewProgress { get; set; }
    public string ReplyProgress { get; set; }
    public string ReviewProgressDisplay { get; set; }
    public string ReplyProgressDisplay { get; set; }
    public string Field_Descname { get; set; } // 審查組別
    
    // Helper methods for display
    public string GetFormattedExpirationDate()
    {
        if (!ExpirationDate.HasValue) return "";
        
        var dateStr = ExpirationDate.Value.ToString("yyyy/MM/dd");
        if (ExpirationDate.Value < DateTime.Now)
        {
            return $"{dateStr} (已屆)";
        }
        return dateStr;
    }
    
    public string GetStatusCssClass()
    {
        if (string.IsNullOrEmpty(StatusesName)) return "status-pending";
        
        switch (StatusesName.ToLower())
        {
            case "審查中":
                return "status-review";
            case "通過":
                return "status-pass";
            case "未通過":
            case "逾期未補":
            case "結案(未通過)":
                return "status-fail";
            case "補正備件":
                return "status-pending";
            default:
                return "status-pending";
        }
    }
    
    public string GetActionButton()
    {
        if (string.IsNullOrEmpty(StatusesName)) return "--";
        
        switch (StatusesName.ToLower())
        {
            case "審查中":
                return "<button class=\"action-btn btn-review\">審查</button>";
            default:
                return "--";
        }
    }
    
    // 取得計畫類別
    public string GetProjectCategory()
    {
        if (string.IsNullOrEmpty(ProjectID)) return "未知";
        
        if (ProjectID.Contains("SCI"))
            return "科專";
        else if (ProjectID.Contains("CUL"))
            return "文化";
        else if (ProjectID.Contains("EDC"))
            return "學校民間";
        else if (ProjectID.Contains("CLB"))
            return "學校社團";
        else
            return "其他";
    }
    
    // 取得狀態/期限顯示文字
    public string GetStatusWithDeadline()
    {
        if (string.IsNullOrEmpty(StatusesName)) return "";
        
        if (ExpirationDate.HasValue && (StatusesName == "補正補件" || StatusesName == "逾期未補"))
        {
            return $"{StatusesName} / {GetFormattedExpirationDate()}";
        }
        
        return StatusesName;
    }
}