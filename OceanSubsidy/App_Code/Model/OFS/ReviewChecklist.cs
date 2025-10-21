using System;
using System.Collections.Generic;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    /// <summary>
    /// 下拉選單項目
    /// </summary>
    public class DropdownItem
    {
        /// <summary>
        /// 選項值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 顯示文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 是否為預設選項
        /// </summary>
        public bool IsSelected { get; set; } = false;
    }

    /// <summary>
    /// 審查清單項目
    /// </summary>
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
        public string Req_SubsidyAmount { get; set; }
        public bool? isWithdrawal { get; set; }
        public bool? isExists { get; set; }
        
        // Progress fields
        public string ReviewProgress { get; set; }
        public string ReplyProgress { get; set; }
        public string ReviewProgressDisplay { get; set; }
        public string ReplyProgressDisplay { get; set; }
        public string Field_Descname { get; set; } // 審查組別
        
        // Type-4 Decision Review fields
        public string Category { get; set; } // 計畫類別 (科專/文化/學校民間/學校社團)
        public string TopicField { get; set; } // 審查組別代碼
        public string ApprovedSubsidy { get; set; } // 核定經費
        public string FinalReviewNotes { get; set; } // 決審意見
        public string TotalScore { get; set; } // 總分
        public string FinalReviewOrder { get; set; } // 決審排序
        
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
                case "審核中":
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
                case "審核中":
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

    /// <summary>
    /// 核定項目資料類別 - 用於核定模式儲存
    /// </summary>
    [Serializable]
    public class ApprovalItem
    {
        /// <summary>
        /// 專案編號
        /// </summary>
        public string ProjectID { get; set; }
        
        /// <summary>
        /// 核定經費
        /// </summary>
        public string ApprovedSubsidy { get; set; }
        
        /// <summary>
        /// 備註
        /// </summary>
        public string FinalReviewNotes { get; set; }
        
        /// <summary>
        /// 計畫類別 (科專/文化/學校民間/學校社團)
        /// </summary>
        public string Category { get; set; }
    }

    /// <summary>
    /// 排序模式項目資料類別 - 用於排序模式顯示和操作
    /// </summary>
    [Serializable]
    public class SortingModeItem
    {
        /// <summary>
        /// 專案編號
        /// </summary>
        public string ProjectID { get; set; }
        
        /// <summary>
        /// 計畫名稱
        /// </summary>
        public string ProjectNameTw { get; set; }
        
        /// <summary>
        /// 申請單位
        /// </summary>
        public string OrgName { get; set; }
        
        /// <summary>
        /// 主持人
        /// </summary>
        public string SupervisoryPersonName { get; set; }
        
        /// <summary>
        /// 總分
        /// </summary>
        public string TotalScore { get; set; }
        
        /// <summary>
        /// 決審排序
        /// </summary>
        public string FinalReviewOrder { get; set; }
        
        /// <summary>
        /// 備註
        /// </summary>
        public string FinalReviewNotes { get; set; }
        
        /// <summary>
        /// 計畫類別 (科專/文化/學校民間/學校社團)
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// 審查組別
        /// </summary>
        public string Field_Descname { get; set; }
        
        /// <summary>
        /// 申請金額
        /// </summary>
        public string Req_SubsidyAmount { get; set; }
        
        /// <summary>
        /// 核定經費
        /// </summary>
        public string ApprovedSubsidy { get; set; }
    }

    /// <summary>
    /// 排序儲存項目資料類別 - 用於排序模式儲存
    /// </summary>
    [Serializable]
    public class SortingSaveItem
    {
        /// <summary>
        /// 專案編號
        /// </summary>
        public string ProjectID { get; set; }
        
        /// <summary>
        /// 決審排序
        /// </summary>
        public int FinalReviewOrder { get; set; }
        
        /// <summary>
        /// 備註
        /// </summary>
        public string FinalReviewNotes { get; set; }
        
        /// <summary>
        /// 計畫類別 (科專/文化/學校民間/學校社團)
        /// </summary>
        public string Category { get; set; }
    }

    #region 內部資料類別

    /// <summary>
    /// 進度資料類別
    /// </summary>
    public class ProgressData
    {
        public string ProjectID { get; set; }
        public int TotalCount { get; set; }
        public int CommentCount { get; set; }
        public int ReplyCount { get; set; }
        public string ReviewProgress { get; set; }
        public string ReplyProgress { get; set; }
        public string ReviewProgressDisplay { get; set; }
        public string ReplyProgressDisplay { get; set; }
    }

    /// <summary>
    /// 審查組別資料類別
    /// </summary>
    public class ReviewGroupData
    {
        public string ProjectID { get; set; }
        public string Field_Descname { get; set; }
    }

    /// <summary>
    /// 批次審核結果類別
    /// </summary>
    [Serializable]
    public class BatchApprovalResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 成功處理的數量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 成功處理的專案編號列表
        /// </summary>
        public List<string> SuccessProjectIds { get; set; } = new List<string>();

        /// <summary>
        /// 錯誤訊息列表
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();

        /// <summary>
        /// 主要訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 處理時間
        /// </summary>
        public DateTime ProcessedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 操作類型
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// 審查類型
        /// </summary>
        public string ReviewType { get; set; }
    }

    /// <summary>
    /// 專案驗證資訊類別
    /// </summary>
    [Serializable]
    public class ProjectValidationInfo
    {
        /// <summary>
        /// 專案編號
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 當前狀態
        /// </summary>
        public string CurrentStatus { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 申請單位
        /// </summary>
        public string OrgName { get; set; }
    }

    /// <summary>
    /// 分頁結果類別
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    [Serializable]
    public class PaginatedResult<T>
    {
        /// <summary>
        /// 當前頁面資料
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// 總記錄數
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// 當前頁碼
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// 當前頁面開始記錄索引
        /// </summary>
        public int StartRecord => (PageNumber - 1) * PageSize + 1;

        /// <summary>
        /// 當前頁面結束記錄索引
        /// </summary>
        public int EndRecord => Math.Min(PageNumber * PageSize, TotalRecords);
    }

    /// <summary>
    /// 計畫變更審核項目資料類別 - 用於 Type=5
    /// </summary>
    [Serializable]
    public class PlanChangeReviewItem
    {
        /// <summary>
        /// 年度
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 計畫編號
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// 類別
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 計畫名稱(中文)
        /// </summary>
        public string ProjectNameTw { get; set; }

        /// <summary>
        /// 申請單位
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 主管單位
        /// </summary>
        public string SupervisoryUnit { get; set; }
        
        /// <summary>
        /// 取得類別顯示名稱 (渲染時使用)
        /// </summary>
        public string GetCategoryDisplayName()
        {
            switch (Category?.ToUpper())
            {
                case "SCI":
                    return "科專";
                case "CUL":
                    return "文化";
                case "EDC":
                    return "學校民間";
                case "CLB":
                    return "學校社團";
                case "MUL":
                    return "多元";
                case "LIT":
                    return "素養";
                case "ACC":
                    return "無障礙";
                default:
                    return Category ?? "";
            }
        }
        
        /// <summary>
        /// 取得主管單位顯示名稱
        /// </summary>
        public string GetSupervisoryUnitDisplayName()
        {
            switch (SupervisoryUnit?.ToUpper())
            {
                case "TECH":
                    return "海洋科技科";
                case "CULTURE":
                    return "海洋文化科";
                case "EDUCATION":
                    return "海洋教育科";
                default:
                    return SupervisoryUnit ?? "";
            }
        }
    }

    /// <summary>
    /// 執行計畫審核項目資料類別 - 用於 Type=6
    /// </summary>
    [Serializable]
    public class ExecutionPlanReviewItem
    {
        /// <summary>
        /// 年度
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 計畫編號
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// 類別 (直接儲存中文名稱如:科專、文化、學校民間、學校社團等)
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 階段
        /// </summary>
        public string Stage { get; set; }

        /// <summary>
        /// 審核待辦事項
        /// </summary>
        public string ReviewTodo { get; set; }

        /// <summary>
        /// 主管人員帳號
        /// </summary>
        public string SupervisoryPersonAccount { get; set; }

        /// <summary>
        /// 主管單位 (完整名稱如:海洋委員會科技文教處海洋科技科)
        /// </summary>
        public string SupervisoryUnit { get; set; }

        /// <summary>
        /// 申請單位
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 計畫名稱(中文)
        /// </summary>
        public string ProjectNameTw { get; set; }

        /// <summary>
        /// 審查委員進度 (僅限科專專案，格式：已繳交人數/總人數 狀態)
        /// </summary>
        public string ReviewProgress { get; set; }
        
        /// <summary>
        /// 取得類別顯示名稱 (渲染時使用)
        /// </summary>
        public string GetCategoryDisplayName()
        {
            switch (Category?.ToUpper())
            {
                case "SCI":
                    return "科專";
                case "CUL":
                    return "文化";
                case "EDC":
                    return "學校民間";
                case "CLB":
                    return "學校社團";
                case "MUL":
                    return "多元";
                case "LIT":
                    return "素養";
                case "ACC":
                    return "無障礙";
                default:
                    return Category ?? "";
            }
        }

        /// <summary>
        /// 取得主管單位簡稱 (用於下拉選單比對)
        /// </summary>
        public string GetSupervisoryUnitShortName()
        {
            if (string.IsNullOrEmpty(SupervisoryUnit)) 
                return "";
            
            if (SupervisoryUnit.Contains("海洋科技科"))
                return "海洋科技科";
            else if (SupervisoryUnit.Contains("海洋文化科"))
                return "海洋文化科";
            else if (SupervisoryUnit.Contains("海洋教育科"))
                return "海洋教育科";
            else
                return SupervisoryUnit;
        }
    }

    #endregion

    #region 審查排名相關資料模型

    /// <summary>
    /// 審查結果排名資料模型
    /// </summary>
    [Serializable]
    public class ReviewRankingItem
    {
        /// <summary>
        /// 密集排名
        /// </summary>
        public int DenseRankNo { get; set; }

        /// <summary>
        /// 專案編號
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjectNameTw { get; set; }

        /// <summary>
        /// 專案總分
        /// </summary>
        public decimal ProjectTotalScore { get; set; }

        /// <summary>
        /// 平均分數
        /// </summary>
        public decimal AvgScore { get; set; }

        /// <summary>
        /// 評審委員分數清單
        /// </summary>
        public List<ReviewerScore> ReviewerScores { get; set; }

        /// <summary>
        /// 建構子
        /// </summary>
        public ReviewRankingItem()
        {
            ReviewerScores = new List<ReviewerScore>();
        }
    }

    /// <summary>
    /// 評審分數資料模型
    /// </summary>
    [Serializable]
    public class ReviewerScore
    {
        /// <summary>
        /// 評審委員名稱
        /// </summary>
        public string ReviewerName { get; set; }

        /// <summary>
        /// 該評審給此專案的分數
        /// </summary>
        public decimal TotalScore { get; set; }

        /// <summary>
        /// 該評審給此專案的排名 (暫時不使用)
        /// </summary>
        // public int ReviewerRank { get; set; }
    }

    #endregion

    #region 批次匯出簡報相關類別

    /// <summary>
    /// 批次簡報匯出結果類別
    /// </summary>
    [Serializable]
    public class BatchPresentationExportResult
    {
        /// <summary>
        /// ZIP檔案路徑
        /// </summary>
        public string ZipFilePath { get; set; }

        /// <summary>
        /// 暫時目錄路徑
        /// </summary>
        public string TempDirectory { get; set; }

        /// <summary>
        /// 找到的檔案數量
        /// </summary>
        public int FoundFileCount { get; set; }

        /// <summary>
        /// 缺少檔案的數量
        /// </summary>
        public int MissingFileCount { get; set; }

        /// <summary>
        /// 缺少檔案的專案清單
        /// </summary>
        public List<string> MissingFiles { get; set; } = new List<string>();

        /// <summary>
        /// 找到的檔案清單
        /// </summary>
        public List<string> FoundFiles { get; set; } = new List<string>();
    }

    #endregion

    public class TaskTemplate
    {
        public string TaskNameEn { get; set; }
        public string TaskName { get; set; }
        public int PriorityLevel { get; set; }
        public bool IsTodo { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? OverdueDate { get; set; }

        public TaskTemplate(string taskNameEn, string taskName, int priorityLevel, bool isTodo, bool isCompleted, DateTime? overdueDate = null)
        {
            TaskNameEn = taskNameEn;
            TaskName = taskName;
            PriorityLevel = priorityLevel;
            IsTodo = isTodo;
            IsCompleted = isCompleted;
            OverdueDate = overdueDate;
        }
    }

    /// <summary>
    /// Type4 (決審核定) 匯出資料項目類別
    /// </summary>
    public class Type4ExportItem
    {
        /// <summary>
        /// 排序
        /// </summary>
        public string 排序 { get; set; } = string.Empty;

        /// <summary>
        /// 年度
        /// </summary>
        public string 年度 { get; set; } = string.Empty;

        /// <summary>
        /// 計畫名稱
        /// </summary>
        public string 計畫名稱 { get; set; } = string.Empty;

        /// <summary>
        /// 申請單位
        /// </summary>
        public string 申請單位 { get; set; } = string.Empty;

        /// <summary>
        /// 總分
        /// </summary>
        public string 總分 { get; set; } = string.Empty;

        /// <summary>
        /// 申請經費
        /// </summary>
        public string 申請經費 { get; set; } = string.Empty;

        /// <summary>
        /// 核定經費
        /// </summary>
        public string 核定經費 { get; set; } = string.Empty;

        /// <summary>
        /// 備註
        /// </summary>
        public string 備註 { get; set; } = string.Empty;
    }
}