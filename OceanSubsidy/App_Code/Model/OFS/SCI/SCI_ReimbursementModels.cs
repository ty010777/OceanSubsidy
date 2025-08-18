using System;

namespace GS.OCA_OceanSubsidy.Model.OFS.SCI
{
    /// <summary>
    /// 期別請款資料模型
    /// </summary>
    public class PhaseReimbursementData
    {
        /// <summary>
        /// 核定經費
        /// </summary>
        public double ApprovedSubsidy { get; set; }

        /// <summary>
        /// 本期請款金額
        /// </summary>
        public double CurrentAmount { get; set; }

        /// <summary>
        /// 前期已撥付金額
        /// </summary>
        public string PreviousAmount { get; set; }

        /// <summary>
        /// 累積實支金額
        /// </summary>
        public string AccumulatedAmount { get; set; }

        /// <summary>
        /// 累積經費執行率
        /// </summary>
        public string ExecutionRate { get; set; }

        /// <summary>
        /// 支用比
        /// </summary>
        public string UsageRatio { get; set; }

        /// <summary>
        /// 階段說明
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 階段名稱
        /// </summary>
        public string PhaseName { get; set; }

        /// <summary>
        /// 累計撥付金額（各期CurrentActualPaidAmount的加總）
        /// </summary>
        public double TotalActualPaidAmount { get; set; }

        /// <summary>
        /// 該期別是否為審核中狀態（顯示審核UI）
        /// </summary>
        public bool IsReimbursementInProgress { get; set; }

        /// <summary>
        /// 目前狀態（請款中/審核中/通過）
        /// </summary>
        public string CurrentStatus { get; set; }

        /// <summary>
        /// 本期實際撥款金額（狀態為通過時使用）
        /// </summary>
        public double CurrentActualPayment { get; set; }

        /// <summary>
        /// 累積實際撥款金額（從第一期到當期的總和，狀態為通過時使用）
        /// </summary>
        public double CumulativeActualPayment { get; set; }
    }

    /// <summary>
    /// 上傳檔案資料模型
    /// </summary>
    public class UploadedFileInfo
    {
        /// <summary>
        /// 檔案代碼
        /// </summary>
        public string FileCode { get; set; }

        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 檔案路徑
        /// </summary>
        public string FilePath { get; set; }
    }
}