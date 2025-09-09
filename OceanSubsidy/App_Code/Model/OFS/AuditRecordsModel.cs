using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 查核紀錄模型類別
/// </summary>
public class AuditRecordsModel
{
    /// <summary>
    /// 計畫基本資料
    /// </summary>
    public class ProjectBasicData
    {
        public string ProjectID { get; set; }
        public string ProjectNameTw { get; set; }
        public string OrgName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// 查核紀錄資料
    /// </summary>
    public class AuditRecordData
    {
        public int idx { get; set; }
        public string ProjectID { get; set; }
        public string ReviewerName { get; set; }
        public DateTime? CheckDate { get; set; }
        public string Risk { get; set; }
        public string ReviewerComment { get; set; }
        public string ExecutorComment { get; set; }
        public DateTime? create_at { get; set; }
        public DateTime? update_at { get; set; }

        /// <summary>
        /// 是否可編輯執行單位回覆
        /// </summary>
        public bool CanEditExecutorComment { get; set; } = false;
    }

    /// <summary>
    /// 查核作業提交資料
    /// </summary>
    public class SubmitAuditData
    {
        public string ProjectID { get; set; }
        public string ReviewerName { get; set; }
        public DateTime? CheckDate { get; set; }
        public string Risk { get; set; }
        public string ReviewerComment { get; set; }
    }

    /// <summary>
    /// 執行單位回覆提交資料
    /// </summary>
    public class SubmitExecutorReplyData
    {
        public int idx { get; set; }
        public string ExecutorComment { get; set; }
    }

    /// <summary>
    /// 用戶權限類型
    /// </summary>
    public enum UserPermissionType
    {
        /// <summary>
        /// 主管單位/系統管理員
        /// </summary>
        Administrator = 1,
        /// <summary>
        /// 一般用戶
        /// </summary>
        GeneralUser = 2
    }
}