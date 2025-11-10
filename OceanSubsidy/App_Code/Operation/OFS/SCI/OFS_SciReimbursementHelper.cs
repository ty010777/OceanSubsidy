using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS.SCI;

/// <summary>
/// SCI 請款核銷相關資料處理 Helper
/// </summary>
public class OFS_SciReimbursementHelper
{
    /// <summary>
    /// 取得期別請款資料
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="phaseOrder">期別序號</param>
    /// <returns>期別請款資料</returns>
    public static PhaseReimbursementData GetPhaseData(string projectID, int phaseOrder)
    {
        if (string.IsNullOrEmpty(projectID))
            throw new ArgumentException("專案ID不可為空");

        DbHelper db = new DbHelper();
        try
        {
            // 檢查該期別的請款狀態
            db.CommandText = @"
                SELECT [Status] 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Payment] 
                WHERE [ProjectID] = @ProjectID AND [Stage] = @PhaseOrder";
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@PhaseOrder", phaseOrder);
            
            DataTable statusDt = db.GetTable();
            bool isReimbursementInProgress = false;
            
            string currentStatus = "";
            if (statusDt.Rows.Count > 0 && statusDt.Rows[0]["Status"] != DBNull.Value)
            {
                currentStatus = statusDt.Rows[0]["Status"].ToString();
                isReimbursementInProgress = currentStatus == "審核中";
            }

            // 取得專案核定經費
            db.CommandText = @"
                SELECT [ApprovedSubsidy] 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] 
                WHERE [ProjectID] = @ProjectID";
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            
            DataTable projectDt = db.GetTable();
            if (projectDt.Rows.Count == 0)
            {
                throw new Exception("找不到專案資料");
            }

            double approvedSubsidy = projectDt.Rows[0]["ApprovedSubsidy"] != DBNull.Value
                ? Math.Round(Convert.ToDouble(projectDt.Rows[0]["ApprovedSubsidy"]))
                : 0;

            // 取得付款階段設定
            db.CommandText = @"
                SELECT [PhaseName], [DisbursementRatioPct], [Note] 
                FROM [OCA_OceanSubsidy].[dbo].[OFS_PaymentPhaseSetting] 
                WHERE [TypeCode] = @TypeCode AND [PhaseOrder] = @PhaseOrder";
            db.Parameters.Clear();
            db.Parameters.Add("@TypeCode", "SCI");
            db.Parameters.Add("@PhaseOrder", phaseOrder);
            
            DataTable phaseDt = db.GetTable();
            
            string phaseName = "";
            decimal? disbursementRatio = null;
            string note = "";
            
            if (phaseDt.Rows.Count > 0)
            {
                DataRow row = phaseDt.Rows[0];
                phaseName = row["PhaseName"]?.ToString() ?? "";
                disbursementRatio = row["DisbursementRatioPct"] != DBNull.Value 
                    ? Convert.ToDecimal(row["DisbursementRatioPct"]) 
                    : (decimal?)null;
                note = row["Note"]?.ToString() ?? "";
            }

            // 取得累計撥付金額
            db.CommandText = "SELECT ISNULL(SUM([CurrentActualPaidAmount]), 0) FROM [OFS_SCI_Payment] WHERE [ProjectID] = @ProjectID";
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            double totalActualPaidAmount = Math.Round(Convert.ToDouble(db.GetTable().Rows[0][0]));

            // 初始化資料
            double currentAmount = 0;
            string previousAmount = "--";
            string accumulatedAmount = "--";
            string executionRate = "--";
            string usageRatio = "--";
            
            if (phaseOrder == 1)
            {
                // 第一期
                currentAmount = CalculateCurrentAmount(approvedSubsidy, phaseOrder, disbursementRatio);
            }
            else if (phaseOrder == 2)
            {
                // 第二期：取得第一期撥付金額
                db.CommandText = "SELECT ISNULL([CurrentActualPaidAmount], 0) FROM [OFS_SCI_Payment] WHERE [ProjectID] = @ProjectID AND [Stage] = 1";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                var result = db.GetTable();
                if (result.Rows.Count > 0)
                {
                    previousAmount = Math.Round(Convert.ToDouble(result.Rows[0][0])).ToString("N0");
                }
                else
                {
                    previousAmount = "0";
                }
                //第二期：累積實支金額
                db.CommandText = "SELECT TotalSpentAmount FROM [OFS_SCI_Payment] WHERE [ProjectID] = @ProjectID AND [Stage] = 2";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                var TotalSpentAmount = db.GetTable();
                if (TotalSpentAmount.Rows.Count > 0 && TotalSpentAmount.Rows[0][0] != DBNull.Value)
                {
                    double totalSpent = Math.Round(Convert.ToDouble(TotalSpentAmount.Rows[0][0]));
                    accumulatedAmount = totalSpent.ToString();
                    currentAmount = Math.Round(totalSpent - Convert.ToDouble(previousAmount.Replace(",", "")));
                }
                else
                {
                    // 第二期沒有資料時，初始化為0
                    accumulatedAmount = "0";
                    currentAmount = 0;
                }
                
                executionRate = "0";
                usageRatio = "0";
            }

            // 計算實際撥款統計（只在狀態為「通過」時計算）
            double currentActualPayment = 0;
            double cumulativeActualPayment = 0;
            
            if (currentStatus == "通過")
            {
                // 取得本期實際撥款金額
                db.CommandText = "SELECT ISNULL([CurrentActualPaidAmount], 0) FROM [OFS_SCI_Payment] WHERE [ProjectID] = @ProjectID AND [Stage] = @PhaseOrder";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                db.Parameters.Add("@PhaseOrder", phaseOrder);
                var currentPaymentResult = db.GetTable();
                if (currentPaymentResult.Rows.Count > 0)
                {
                    currentActualPayment = Math.Round(Convert.ToDouble(currentPaymentResult.Rows[0][0]));
                }

                // 計算累積實際撥款金額（從第一期到當期）
                db.CommandText = "SELECT ISNULL(SUM([CurrentActualPaidAmount]), 0) FROM [OFS_SCI_Payment] WHERE [ProjectID] = @ProjectID AND [Stage] <= @PhaseOrder";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                db.Parameters.Add("@PhaseOrder", phaseOrder);
                var cumulativePaymentResult = db.GetTable();
                if (cumulativePaymentResult.Rows.Count > 0)
                {
                    cumulativeActualPayment = Math.Round(Convert.ToDouble(cumulativePaymentResult.Rows[0][0]));
                }
            }

            var PhaseReimbursementData = new PhaseReimbursementData
            {
                ApprovedSubsidy = approvedSubsidy,
                CurrentAmount = currentAmount,
                PreviousAmount = previousAmount,
                AccumulatedAmount = accumulatedAmount,
                ExecutionRate = executionRate,
                UsageRatio = usageRatio,
                Note = note,
                PhaseName = phaseName,
                TotalActualPaidAmount = totalActualPaidAmount, // 傳回累計撥付金額供前端使用
                IsReimbursementInProgress = isReimbursementInProgress, // 新增：該期別是否為審核中狀態
                CurrentStatus = currentStatus, // 傳回目前狀態供前端判斷
                CurrentActualPayment = currentActualPayment, // 本期實際撥款
                CumulativeActualPayment = cumulativeActualPayment // 累積實際撥款
            };

            return PhaseReimbursementData;
        }
        finally
        {
            if (db != null)
                db.Dispose();
        }
    }

    /// <summary>
    /// 計算本期請款金額
    /// </summary>
    /// <param name="approvedSubsidy">核定經費</param>
    /// <param name="phaseOrder">期別序號</param>
    /// <param name="disbursementRatio">撥付比例</param>
    /// <returns>本期請款金額</returns>
    private static double CalculateCurrentAmount(double approvedSubsidy, int phaseOrder, decimal? disbursementRatio)
    {
        if (phaseOrder == 1 && disbursementRatio.HasValue)
        {
            // 第一期：核定經費 * 撥付比例，四捨五入到整數
            return Math.Round(approvedSubsidy * (double)(disbursementRatio.Value / 100));
        }
        return 0;
    }

    /// <summary>
    /// 提送請款，更新狀態為"請款中"並處理OFS_SCI_Payment資料
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="phaseOrder">期別序號</param>
    /// <returns>是否成功</returns>
    public static void SubmitReimbursement(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
            throw new ArgumentException("專案ID不可為空");

        DbHelper db = new DbHelper();
        try
        {
            // 1. 更新OFS_SCI_Project_Main的StatusesName為"審核中"
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] 
                SET [StatusesName] = @StatusesName, 
                    [updated_at] = @updated_at 
                WHERE [ProjectID] = @ProjectID";
                
            db.Parameters.Clear();
            db.Parameters.Add("@StatusesName", "審核中");
            db.Parameters.Add("@updated_at", DateTime.Now);
            db.Parameters.Add("@ProjectID", projectID);
            
            db.ExecuteNonQuery();

            
        }
        finally
        {
            if (db != null)
                db.Dispose();
        }
    }

    /// <summary>
    /// 處理OFS_SCI_Payment資料的新增或更新
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="phaseOrder">期別序號</param>
    /// <param name="currentRequestAmount">本期請款金額(從前端傳入)</param>
    /// <param name="accumulatedAmount">累積實支金額(第二期使用)</param>
    /// <param name="status">狀態(請款中/審核中)</param>
    public static void ProcessPaymentData(string projectID, int phaseOrder, decimal currentRequestAmount, decimal accumulatedAmount = 0, string status = "審核中")
    {
        using (DbHelper db = new DbHelper())
        {
            // 取得撥付比例
            db.CommandText = "SELECT [DisbursementRatioPct] FROM [OFS_PaymentPhaseSetting] WHERE [TypeCode] = 'SCI' AND [PhaseOrder] = @PhaseOrder";
            db.Parameters.Clear();
            db.Parameters.Add("@PhaseOrder", phaseOrder);
            var ratioResult = db.GetTable();
            decimal? ratio = null;
            if (!string.IsNullOrEmpty(ratioResult.Rows[0]["DisbursementRatioPct"].ToString()))
            {
                ratio =Convert.ToDecimal(ratioResult.Rows[0][0]);
            }
           
            

            // 檢查是否存在
            db.CommandText = "SELECT COUNT(*) FROM [OFS_SCI_Payment] WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@Stage", phaseOrder);
            bool exists = Convert.ToInt32(db.GetTable().Rows[0][0]) > 0;

            // 準備參數
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@Stage", phaseOrder);
            db.Parameters.Add("@ActDisbursementRatioPct", ratio);
            db.Parameters.Add("@CurrentRequestAmount", currentRequestAmount);
            db.Parameters.Add("@Status", status);
            db.Parameters.Add("@UpdateTime", DateTime.Now);

            if (exists)
            {
                // 更新
                db.CommandText = "UPDATE [OFS_SCI_Payment] SET [ActDisbursementRatioPct] = @ActDisbursementRatioPct, [CurrentRequestAmount] = @CurrentRequestAmount, [Status] = @Status, [UpdateTime] = @UpdateTime";
                if (phaseOrder >= 2) // 第二期以後才更新TotalSpentAmount
                {
                    db.CommandText += ", [TotalSpentAmount] = @TotalSpentAmount";
                    db.Parameters.Add("@TotalSpentAmount", accumulatedAmount);
                }
                db.CommandText += " WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
            }
            else
            {
                // 新增
                db.Parameters.Add("@CreateTime", DateTime.Now);
                if (phaseOrder >= 2) // 第二期以後才包含TotalSpentAmount
                {
                    db.CommandText = "INSERT INTO [OFS_SCI_Payment] ([ProjectID], [Stage], [ActDisbursementRatioPct], [TotalSpentAmount], [CurrentRequestAmount], [Status], [CreateTime], [UpdateTime]) VALUES (@ProjectID, @Stage, @ActDisbursementRatioPct, @TotalSpentAmount, @CurrentRequestAmount, @Status, @CreateTime, @UpdateTime)";
                    db.Parameters.Add("@TotalSpentAmount", accumulatedAmount);
                }
                else
                {
                    db.CommandText = "INSERT INTO [OFS_SCI_Payment] ([ProjectID], [Stage], [ActDisbursementRatioPct], [CurrentRequestAmount], [Status], [CreateTime], [UpdateTime]) VALUES (@ProjectID, @Stage, @ActDisbursementRatioPct, @CurrentRequestAmount, @Status, @CreateTime, @UpdateTime)";
                }
            }
            
            db.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 取得付款階段設定清單
    /// </summary>
    /// <param name="typeCode">類型代碼</param>
    /// <returns>付款階段設定清單</returns>
    public static List<OFS_PaymentPhaseSetting> GetPaymentPhaseSettings(string typeCode)
    {
        List<OFS_PaymentPhaseSetting> result = new List<OFS_PaymentPhaseSetting>();
        
        if (string.IsNullOrEmpty(typeCode))
            return result;

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT [ID], [TypeCode], [PhaseOrder], [PhaseName], [DisbursementRatioPct], [DisbursementType], [Note]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_PaymentPhaseSetting] 
                WHERE [TypeCode] = @TypeCode 
                ORDER BY [PhaseOrder]";
            db.Parameters.Clear();
            db.Parameters.Add("@TypeCode", typeCode);
            
            DataTable dt = db.GetTable();
            
            foreach (DataRow row in dt.Rows)
            {
                var setting = new OFS_PaymentPhaseSetting
                {
                    ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                    TypeCode = row["TypeCode"]?.ToString() ?? "",
                    PhaseOrder = row["PhaseOrder"] != DBNull.Value ? Convert.ToInt32(row["PhaseOrder"]) : (int?)null,
                    PhaseName = row["PhaseName"]?.ToString() ?? "",
                    DisbursementRatioPct = row["DisbursementRatioPct"] != DBNull.Value ? Convert.ToDecimal(row["DisbursementRatioPct"]) : (decimal?)null,
                    DisbursementType = row["DisbursementType"]?.ToString() ?? "",
                    Note = row["Note"]?.ToString() ?? ""
                };
                result.Add(setting);
            }
            
            return result;
        }
        finally
        {
            if (db != null)
                db.Dispose();
        }
    }
    public static void UpdatePayment(
        string projectID, 
        string phaseOrder, 
        decimal currentActualPaidAmount,
        string status,
        string reviewerComment,
        string reviewUser)
    {
        if (string.IsNullOrEmpty(projectID))
            throw new ArgumentException("專案ID不可為空");

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Payment]
            SET 
                [CurrentActualPaidAmount] = @CurrentActualPaidAmount,
                [ReviewerComment] = @ReviewerComment,
                [Status] = @Status,
                [ReviewUser] = @ReviewUser,
                [ReviewTime] = @ReviewTime,
                [UpdateTime] = @UpdateTime
            WHERE 
                [ProjectID] = @ProjectID
                AND [Stage] = @Stage";

            db.Parameters.Clear();
            db.Parameters.Add("@CurrentActualPaidAmount", currentActualPaidAmount);
            db.Parameters.Add("@ReviewerComment", reviewerComment ?? (object)DBNull.Value);
            db.Parameters.Add("@Status", status);
            db.Parameters.Add("@ReviewUser", reviewUser ?? (object)DBNull.Value);
            db.Parameters.Add("@ReviewTime", DateTime.Now);
            db.Parameters.Add("@UpdateTime", DateTime.Now);
            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@Stage", phaseOrder);

            db.ExecuteNonQuery();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 根據ProjectID查詢組織類型
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>組織類型</returns>
    public static string GetOrgCategoryByProjectID(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
            return "";

        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                    SELECT [OrgCategory] 
                    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] 
                    WHERE [ProjectID] = @ProjectID";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                
                var result = db.GetTable();
                if (result.Rows.Count > 0 && result.Rows[0]["OrgCategory"] != DBNull.Value)
                {
                    return result.Rows[0]["OrgCategory"].ToString();
                }
                
                return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"查詢組織類型時發生錯誤: {ex.Message}");
                return "";
            }
        }
    }

    /// <summary>
    /// 儲存上傳檔案記錄
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <param name="fileName">檔案名稱</param>
    /// <param name="filePath">檔案路徑</param>
    /// <returns>是否成功</returns>
    public static bool SaveUploadedFile(string projectID, string fileCode, string fileName, string filePath)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 檢查是否已存在相同檔案代碼的記錄
                db.CommandText = "SELECT COUNT(*) FROM [OFS_SCI_UploadFile] WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                db.Parameters.Add("@FileCode", fileCode);
                
                bool exists = Convert.ToInt32(db.GetTable().Rows[0][0]) > 0;

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                db.Parameters.Add("@FileCode", fileCode);
                db.Parameters.Add("@FileName", fileName);
                db.Parameters.Add("@TemplatePath", filePath);

                if (exists)
                {
                    // 更新現有記錄 - 直接覆蓋
                    db.CommandText = @"
                        UPDATE [OFS_SCI_UploadFile] 
                        SET [FileName] = @FileName, 
                            [TemplatePath] = @TemplatePath
                        WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";
                }
                else
                {
                    // 新增記錄
                    db.CommandText = @"
                        INSERT INTO [OFS_SCI_UploadFile] 
                        ([ProjectID], [FileCode], [FileName], [TemplatePath]) 
                        VALUES (@ProjectID, @FileCode, @FileName, @TemplatePath)";
                }

                db.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"儲存上傳檔案記錄時發生錯誤: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// 取得專案的已上傳檔案清單
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>已上傳檔案清單</returns>
    public static List<UploadedFileInfo> GetUploadedFiles(string projectID)
    {
        var result = new List<UploadedFileInfo>();
        
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                    SELECT [FileCode], [FileName], [TemplatePath]
                    FROM [OFS_SCI_UploadFile] 
                    WHERE [ProjectID] = @ProjectID
                    ORDER BY [FileCode]";
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);
                
                DataTable dt = db.GetTable();
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new UploadedFileInfo
                    {
                        FileCode = row["FileCode"]?.ToString() ?? "",
                        FileName = row["FileName"]?.ToString() ?? "",
                        FilePath = row["TemplatePath"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得已上傳檔案清單時發生錯誤: {ex.Message}");
            }
        }
        
        return result;
    }

    /// <summary>
    /// 產生檔案儲存路徑
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="originalFileName">原始檔名</param>
    /// <param name="fileCode">檔案代碼</param>
    /// <returns>檔案儲存路徑</returns>
    public static string GenerateFilePath(string projectID, string originalFileName, string fileCode)
    {
        string extension = System.IO.Path.GetExtension(originalFileName);
        string fileName = "";

        if (fileCode == "REIMBURSE_EXPENSE")
        {
            fileName = $"{projectID}_第二期請款_經費支用表及明細表{extension}";
        }
        else if (fileCode == "REIMBURSE_RECEIPT")
        {
            fileName = $"{projectID}_第二期請款_憑證{extension}";
        }
        else
        {
            fileName = $"{projectID}_{fileCode}{extension}";
        }

        return $"UploadFiles/OFS/SCI/{projectID}/{fileName}";
    }

    /// <summary>
    /// 更新專案狀態為「已結案」
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    public static void UpdateProjectStatusToClosed(string projectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
            SET
                [StatusesName] = @StatusesName,
                [updated_at] = GETDATE()
            WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@StatusesName", "已結案");

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 取得當前應顯示的期別（1 或 2）
    /// 邏輯：如果第一期請款狀態為「通過」，返回第二期；否則返回第一期
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>當前期別（1 或 2）</returns>
    public static int GetCurrentPhase(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
            return 1;

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT TOP(1) Status
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Payment]
                WHERE ProjectID = @ProjectID
                  AND Stage = 1
                ORDER BY ID DESC";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectID);

            DataTable dt = db.GetTable();

            // 如果第一期狀態為「通過」，返回第二期；否則返回第一期
            if (dt.Rows.Count > 0 && dt.Rows[0]["Status"] != DBNull.Value)
            {
                string status = dt.Rows[0]["Status"].ToString();
                if (status == "通過")
                {
                    return 2;
                }
            }

            return 1;
        }
        finally
        {
            if (db != null)
                db.Dispose();
        }
    }

    /// <summary>
    /// 從上傳的經費支用表 Excel 讀取累積實支金額
    /// </summary>
    /// <param name="filePath">Excel 檔案實體路徑</param>
    /// <param name="projectID">專案ID（用於判斷組織類型）</param>
    /// <returns>累積實支金額，如果讀取失敗則返回 null（不會拋出錯誤）</returns>
    public static decimal? ReadAccumulatedAmountFromExcel(string filePath, string projectID)
    {
        try
        {
            // 1. 判斷檔案是否為 Excel
            string extension = System.IO.Path.GetExtension(filePath).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                System.Diagnostics.Debug.WriteLine("檔案格式不正確，必須為 Excel 檔案");
                return null;
            }

            // 2. 取得組織類型
            string orgCategory = GetOrgCategoryByProjectID(projectID);
            if (string.IsNullOrEmpty(orgCategory))
            {
                System.Diagnostics.Debug.WriteLine("無法取得專案的組織類型");
                return null;
            }

            // 3. 根據組織類型讀取不同的工作表和儲存格
            string sheetName;
            string columnLetter;
            int rowNumber;

            if (orgCategory == "OceanTech")
            {
                // 業者：讀取「計畫經費支用彙總表」工作表的 K14 儲存格
                sheetName = "計畫經費支用彙總表";
                columnLetter = "K";
                rowNumber = 14;
            }
            else if (orgCategory == "Academic" || orgCategory == "Legal")
            {
                // 學研/法人：讀取「經費支用明細表」工作表的 C23 儲存格
                sheetName = "經費支用明細表";
                columnLetter = "C";
                rowNumber = 23;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"不支援的組織類型: {orgCategory}");
                return null;
            }

            // 4. 使用 NPOIHelper 讀取儲存格值
            decimal? amount = NPOIHelper.ReadCellValue(filePath, sheetName, columnLetter, rowNumber);

            // 5. 驗證是否為有效數字
            if (!amount.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"無法讀取儲存格 {columnLetter}{rowNumber} 的值（可能工作表名稱或位置有變更）");
                return null;
            }

            if (amount.Value < 0)
            {
                System.Diagnostics.Debug.WriteLine($"儲存格 {columnLetter}{rowNumber} 的值為負數，不合理");
                return null;
            }

            return amount;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"讀取 Excel 累積實支金額時發生錯誤: {ex.Message}");
            return null; // 不拋出例外，返回 null
        }
    }

}
