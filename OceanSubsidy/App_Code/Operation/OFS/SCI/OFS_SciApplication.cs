using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.App;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class OFS_SciApplicationHelper
{
    public OFS_SciApplicationHelper()
    {

    }

    /// <summary>
    /// 計算同單位申請計畫數
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="orgName">執行單位名稱（選填）</param>
    /// <returns>計畫總數</returns>
    public static int count(int year, string orgName = "")
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT COUNT(*) AS Count
              FROM [OFS_SCI_Application_Main]
             WHERE [Year] = @Year
        ";

        db.Parameters.Add("@Year", year);

        if (!string.IsNullOrWhiteSpace(orgName))
        {
            db.CommandText += " AND [OrgName] = @OrgName";
            db.Parameters.Add("@OrgName", orgName);
        }

        return int.Parse(db.GetTable().Rows[0]["Count"].ToString());
    }

    public static void insertApplicationMain(OFS_SCI_Application_Main applicationData)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
(
    [ProjectID],
    [PersonID],
    [Year],
    [Serial],
    [SubsidyPlanType],
    [ProjectNameTw],
    [ProjectNameEn],
    [OrgCategory],
    [Topic],
    [Field],
    [CountryTech_Underwater],
    [CountryTech_Geology],
    [CountryTech_Physics],
    [OrgName],
    [OrgPartner],
    [RegisteredAddress],
    [CorrespondenceAddress],
    [Target],
    [Summary],
    [Innovation],
    [Declaration],
    [IsRecused],
    [StartTime],
    [EndTime],
    [created_at],
    [updated_at],
    [isExist]
)
VALUES
(
    @ProjectID,              -- 計畫編號 (年度 + 流水號4碼)
    @PersonID,               -- 人員申請表外鍵
    @Year,                   -- 年度
    @Serial,                 -- 流水序號
    @SubsidyPlanType,        -- 補助計畫類別
    @ProjectNameTw,          -- 計畫名稱(中文)
    @ProjectNameEn,          -- 計畫名稱(英文)
    @OrgCategory,            -- 申請單位類別
    @Topic,                  -- 主題
    @Field,                  -- 領域
    @CountryTech_Underwater, -- 國家核心科技 水下研究
    @CountryTech_Geology,    -- 國家核心科技 海洋地質
    @CountryTech_Physics,    -- 國家核心科技 海洋物理
    @OrgName,                -- 申請單位
    @OrgPartner,             -- 共同執行單位
    @RegisteredAddress,      -- 登記地址
    @CorrespondenceAddress,  -- 通訊地址
    @Target,                 -- 計畫目標
    @Summary,                -- 計畫內容摘要
    @Innovation,             -- 創新重點
    @Declaration,            -- 聲明同意書
    @IsRecused,              -- 是否有勾選迴避委員
    @StartTime,              -- 工作項目的計畫期程 - 開始
    @EndTime,                -- 工作項目的計畫期程 - 結束
    @created_at,             -- 建立時間
    @updated_at,             -- 更新時間
    @isExist                 -- 是否存在
)";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", applicationData.ProjectID);
        db.Parameters.Add("@PersonID", applicationData.PersonID);
        db.Parameters.Add("@Year", applicationData.Year);
        db.Parameters.Add("@Serial", applicationData.Serial);
        db.Parameters.Add("@SubsidyPlanType", applicationData.SubsidyPlanType);
        db.Parameters.Add("@ProjectNameTw", applicationData.ProjectNameTw);
        db.Parameters.Add("@ProjectNameEn", applicationData.ProjectNameEn);
        db.Parameters.Add("@OrgCategory", applicationData.OrgCategory);
        db.Parameters.Add("@Topic", applicationData.Topic);
        db.Parameters.Add("@Field", applicationData.Field);
        db.Parameters.Add("@CountryTech_Underwater", applicationData.CountryTech_Underwater);
        db.Parameters.Add("@CountryTech_Geology", applicationData.CountryTech_Geology);
        db.Parameters.Add("@CountryTech_Physics", applicationData.CountryTech_Physics);
        db.Parameters.Add("@OrgName", applicationData.OrgName);
        db.Parameters.Add("@OrgPartner", applicationData.OrgPartner);
        db.Parameters.Add("@RegisteredAddress", applicationData.RegisteredAddress);
        db.Parameters.Add("@CorrespondenceAddress", applicationData.CorrespondenceAddress);
        db.Parameters.Add("@Target", applicationData.Target);
        db.Parameters.Add("@Summary", applicationData.Summary);
        db.Parameters.Add("@Innovation", applicationData.Innovation);
        db.Parameters.Add("@Declaration", applicationData.Declaration);
        db.Parameters.Add("@IsRecused", applicationData.IsRecused);
        db.Parameters.Add("@StartTime", applicationData.StartTime);
        db.Parameters.Add("@EndTime", applicationData.EndTime);
        db.Parameters.Add("@created_at", applicationData.created_at ?? DateTime.Now);
        db.Parameters.Add("@updated_at", applicationData.updated_at ?? DateTime.Now);
        db.Parameters.Add("@isExist", applicationData.isExist ?? true);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // 記錄錯誤或重新拋出例外
            throw new Exception($"插入申請主檔時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            // 確保資源被釋放
            db.Dispose();
        }
    }

    public static void updateApplicationMain(OFS_SCI_Application_Main applicationData)
    {
        using (DbHelper db = new DbHelper())
        {
            var setClauses = new List<string>();
            db.Parameters.Clear();

            // Helper 函式：只有有值才加入更新
            void AddIfNotNull(string column, object value)
            {
                if (value != null && !(value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    setClauses.Add($"[{column}] = @{column}");
                    db.Parameters.Add("@" + column, value);
                }
            }

            // 檢查並加入欄位
            AddIfNotNull("PersonID", applicationData.PersonID);
            AddIfNotNull("Year", applicationData.Year);
            AddIfNotNull("Serial", applicationData.Serial);
            AddIfNotNull("SubsidyPlanType", applicationData.SubsidyPlanType);
            AddIfNotNull("ProjectNameTw", applicationData.ProjectNameTw);
            AddIfNotNull("ProjectNameEn", applicationData.ProjectNameEn);
            AddIfNotNull("OrgCategory", applicationData.OrgCategory);
            AddIfNotNull("Topic", applicationData.Topic);
            AddIfNotNull("Field", applicationData.Field);
            AddIfNotNull("CountryTech_Underwater", applicationData.CountryTech_Underwater);
            AddIfNotNull("CountryTech_Geology", applicationData.CountryTech_Geology);
            AddIfNotNull("CountryTech_Physics", applicationData.CountryTech_Physics);
            AddIfNotNull("OrgName", applicationData.OrgName);
            AddIfNotNull("OrgPartner", applicationData.OrgPartner);
            AddIfNotNull("RegisteredAddress", applicationData.RegisteredAddress);
            AddIfNotNull("CorrespondenceAddress", applicationData.CorrespondenceAddress);
            AddIfNotNull("Target", applicationData.Target);
            AddIfNotNull("Summary", applicationData.Summary);
            AddIfNotNull("Innovation", applicationData.Innovation);
            AddIfNotNull("Declaration", applicationData.Declaration);
            AddIfNotNull("IsRecused", applicationData.IsRecused);
            AddIfNotNull("StartTime", applicationData.StartTime);
            AddIfNotNull("EndTime", applicationData.EndTime);
            AddIfNotNull("created_at", applicationData.created_at);
            AddIfNotNull("updated_at", applicationData.updated_at);
            AddIfNotNull("isExist", applicationData.isExist);

            if (setClauses.Count == 0)
            {
                throw new Exception("沒有提供任何欄位可供更新。");
            }

            // 加上 WHERE ProjectID
            db.Parameters.Add("@ProjectID", applicationData.ProjectID);

            // 組 SQL
            db.CommandText = $@"
            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
            SET {string.Join(",\n    ", setClauses)}
            WHERE [ProjectID] = @ProjectID";

            try
            {
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新申請主檔時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    // 根據 ProjectID 查詢單筆資料

    public static OFS_SCI_Application_Main getApplicationMainByProjectID(string ProjectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"SELECT *
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] 
    WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", ProjectID);

        try
        {
            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_SCI_Application_Main
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    PersonID = row["PersonID"]?.ToString(),
                    Year = row["Year"] != DBNull.Value ? Convert.ToInt32(row["Year"]) : (int?)null,
                    Serial = row["Serial"] != DBNull.Value ? Convert.ToInt32(row["Serial"]) : (int?)null,
                    SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    ProjectNameEn = row["ProjectNameEn"]?.ToString(),
                    OrgCategory = row["OrgCategory"]?.ToString(),
                    Topic = row["Topic"]?.ToString(),
                    Field = row["Field"]?.ToString(),
                    CountryTech_Underwater = row["CountryTech_Underwater"] != DBNull.Value
                        ? (bool?)row["CountryTech_Underwater"]
                        : null,
                    CountryTech_Geology = row["CountryTech_Geology"] != DBNull.Value
                        ? (bool?)row["CountryTech_Geology"]
                        : null,
                    CountryTech_Physics = row["CountryTech_Physics"] != DBNull.Value
                        ? (bool?)row["CountryTech_Physics"]
                        : null,
                    OrgName = row["OrgName"]?.ToString(),
                    OrgPartner = row["OrgPartner"]?.ToString(),
                    RegisteredAddress = row["RegisteredAddress"]?.ToString(),
                    CorrespondenceAddress = row["CorrespondenceAddress"]?.ToString(),
                    Target = row["Target"]?.ToString(),
                    Summary = row["Summary"]?.ToString(),
                    Innovation = row["Innovation"]?.ToString(),
                    Declaration = row["Declaration"] != DBNull.Value ? (bool?)row["Declaration"] : null,
                    IsRecused = row["IsRecused"] != DBNull.Value ? (bool?)row["IsRecused"] : null,
                    StartTime = row["StartTime"] != DBNull.Value
                        ? Convert.ToDateTime(row["StartTime"])
                        : (DateTime?)null,
                    EndTime = row["EndTime"] != DBNull.Value
                        ? Convert.ToDateTime(row["EndTime"])
                        : (DateTime?)null,
                    created_at = row["created_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["created_at"])
                        : (DateTime?)null,
                    updated_at = row["updated_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["updated_at"])
                        : (DateTime?)null,
                    isExist = row["isExist"] != DBNull.Value ? (bool?)row["isExist"] : null
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢申請主檔時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    public static Tuple<string, string> GetProjectPerson(string ProjectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"SELECT [ProjectID], [PersonID]
                       FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] 
                       WHERE [ProjectID] = @ProjectID";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", ProjectID);

        try
        {
            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return Tuple.Create(
                    row["ProjectID"]?.ToString(),
                    row["PersonID"]?.ToString()
                );
            }

            return null; // 查無資料回傳 null
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢申請主檔時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    public static OFS_SCI_Application_Main getLatestApplicationMain(string Year)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT TOP(1) * 
        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
        where Year = @Year
        order by Serial Desc ";

        db.Parameters.Clear();
        db.Parameters.Add("@Year", Year);

        try
        {
            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new OFS_SCI_Application_Main
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    PersonID = row["PersonID"]?.ToString(),
                    Year = row["Year"] != DBNull.Value ? Convert.ToInt32(row["Year"]) : (int?)null,
                    Serial = row["Serial"] != DBNull.Value ? Convert.ToInt32(row["Serial"]) : (int?)null,
                    SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    ProjectNameEn = row["ProjectNameEn"]?.ToString(),
                    OrgCategory = row["OrgCategory"]?.ToString(),
                    Topic = row["Topic"]?.ToString(),
                    Field = row["Field"]?.ToString(),
                    CountryTech_Underwater = row["CountryTech_Underwater"] != DBNull.Value
                        ? (bool?)row["CountryTech_Underwater"]
                        : null,
                    CountryTech_Geology = row["CountryTech_Geology"] != DBNull.Value
                        ? (bool?)row["CountryTech_Geology"]
                        : null,
                    CountryTech_Physics = row["CountryTech_Physics"] != DBNull.Value
                        ? (bool?)row["CountryTech_Physics"]
                        : null,
                    OrgName = row["OrgName"]?.ToString(),
                    OrgPartner = row["OrgPartner"]?.ToString(),
                    RegisteredAddress = row["RegisteredAddress"]?.ToString(),
                    CorrespondenceAddress = row["CorrespondenceAddress"]?.ToString(),
                    Target = row["Target"]?.ToString(),
                    Summary = row["Summary"]?.ToString(),
                    Innovation = row["Innovation"]?.ToString(),
                    Declaration = row["Declaration"] != DBNull.Value ? (bool?)row["Declaration"] : null,
                    IsRecused = row["IsRecused"] != DBNull.Value ? (bool?)row["IsRecused"] : null,
                    StartTime = row["StartTime"] != DBNull.Value
                        ? Convert.ToDateTime(row["StartTime"])
                        : (DateTime?)null,
                    EndTime = row["EndTime"] != DBNull.Value
                        ? Convert.ToDateTime(row["EndTime"])
                        : (DateTime?)null,
                    created_at = row["created_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["created_at"])
                        : (DateTime?)null,
                    updated_at = row["updated_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["updated_at"])
                        : (DateTime?)null,
                    isExist = row["isExist"] != DBNull.Value ? (bool?)row["isExist"] : null
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢申請主檔時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    public static void SavePersonnel(List<OFS_SCI_Application_Personnel> personList)
    {
        foreach (var person in personList)
        {
            using (DbHelper db = new DbHelper())
            {
                if (person.idx == 0)
                {
                    db.CommandText = @"
                    INSERT INTO OFS_SCI_Application_Personnel 
                    (PersonID, Role, Name, JobTitle, Phone, PhoneExt, MobilePhone)
                    VALUES
                    (@PersonID, @Role, @Name, @JobTitle, @Phone, @PhoneExt, @MobilePhone)";
                }
                else
                {
                    db.CommandText = @"
                    UPDATE OFS_SCI_Application_Personnel 
                    SET PersonID = @PersonID,
                        Role = @Role,
                        Name = @Name,
                        JobTitle = @JobTitle,
                        Phone = @Phone,
                        PhoneExt = @PhoneExt,
                        MobilePhone = @MobilePhone
                    WHERE idx = @idx";
                }

                db.Parameters.Clear();
                db.Parameters.Add("@PersonID", person.PersonID);
                db.Parameters.Add("@Role", person.Role);
                db.Parameters.Add("@Name", person.Name);
                db.Parameters.Add("@JobTitle", person.JobTitle);
                db.Parameters.Add("@Phone", person.Phone);
                db.Parameters.Add("@PhoneExt", person.PhoneExt);
                db.Parameters.Add("@MobilePhone", person.MobilePhone);
                if (person.idx != 0)
                {
                    db.Parameters.Add("@idx", person.idx);
                }

                db.ExecuteNonQuery();
            }
        }
    }

    public static List<OFS_SCI_Application_Personnel> GetPersonnelByPersonID(string personID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT idx, PersonID, Role, Name, JobTitle, Phone, PhoneExt, MobilePhone
        FROM OFS_SCI_Application_Personnel
        WHERE PersonID = @PersonID
        ORDER BY idx"; // 可加排序

        db.Parameters.Clear();
        db.Parameters.Add("@PersonID", personID);

        try
        {
            DataTable dt = db.GetTable();
            List<OFS_SCI_Application_Personnel> personnelList = new List<OFS_SCI_Application_Personnel>();

            foreach (DataRow row in dt.Rows)
            {
                var personnel = new OFS_SCI_Application_Personnel
                {
                    idx = row["idx"] != DBNull.Value ? Convert.ToInt32(row["idx"]) : 0,
                    PersonID = row["PersonID"]?.ToString(),
                    Role = row["Role"]?.ToString(),
                    Name = row["Name"]?.ToString(),
                    JobTitle = row["JobTitle"]?.ToString(),
                    Phone = row["Phone"]?.ToString(),
                    PhoneExt = row["PhoneExt"]?.ToString(),
                    MobilePhone = row["MobilePhone"]?.ToString()
                };

                personnelList.Add(personnel);
            }

            return personnelList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢人員資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    public static void SaveKeywordsToDatabase(string keywordID, List<OFS_SCI_Application_KeyWord> keywordList)
    {
        using (DbHelper db = new DbHelper())
        {
            // 新增關鍵字
            db.CommandText = @"
                Delete FROM OFS_SCI_Application_KeyWord
                WHERE KeywordID = @KeywordID";
            db.Parameters.Clear();
            db.Parameters.Add("@KeywordID", keywordID);
            db.ExecuteNonQuery();
        }


        foreach (var keyword in keywordList)
        {
            // 跳過空白的關鍵字
            if (string.IsNullOrWhiteSpace(keyword.KeyWordTw) && string.IsNullOrWhiteSpace(keyword.KeyWordEn))
                continue;

            using (DbHelper db = new DbHelper())
            {
                // 新增關鍵字
                db.CommandText = @"
                INSERT INTO [OFS_SCI_Application_KeyWord] 
                ([KeywordID], [KeyWordTw], [KeyWordEn])
                VALUES
                (@KeywordID, @KeyWordTw, @KeyWordEn)";


                db.Parameters.Clear();
                db.Parameters.Add("@KeywordID", keyword.KeywordID);
                db.Parameters.Add("@KeyWordTw", keyword.KeyWordTw ?? "");
                db.Parameters.Add("@KeyWordEn", keyword.KeyWordEn ?? "");

                if (keyword.Idx != 0)
                {
                    db.Parameters.Add("@Idx", keyword.Idx);
                }

                db.ExecuteNonQuery();
            }
        }
    }

    public static List<OFS_SCI_Application_KeyWord> GetKeywordsByID(string KeywordID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"SELECT *
    FROM OFS_SCI_Application_KeyWord
    WHERE KeywordID = @KeywordID
    ORDER BY Idx";

        db.Parameters.Clear();
        db.Parameters.Add("@KeywordID", KeywordID);

        try
        {
            DataTable dt = db.GetTable();
            List<OFS_SCI_Application_KeyWord> keywordList = new List<OFS_SCI_Application_KeyWord>();

            foreach (DataRow row in dt.Rows)
            {
                var Kw = new OFS_SCI_Application_KeyWord
                {
                    Idx = row["idx"] != DBNull.Value ? Convert.ToInt32(row["idx"]) : 0,
                    KeywordID = row["KeywordID"]?.ToString(),
                    KeyWordTw = row["KeyWordTw"]?.ToString(),
                    KeyWordEn = row["KeyWordEn"]?.ToString(),
                };

                keywordList.Add(Kw);
            }

            return keywordList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢關鍵字資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    public static List<Sys_ZgsCode> GetSysZgsCodeByCodeGroup(string codeGroup)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT *
        FROM Sys_ZgsCode
        WHERE CodeGroup = @CodeGroup AND IsValid = '1'
        ORDER BY OrderNo";

        db.Parameters.Clear();
        db.Parameters.Add("@CodeGroup", codeGroup);

        try
        {
            DataTable dt = db.GetTable();
            List<Sys_ZgsCode> codeList = new List<Sys_ZgsCode>();

            foreach (DataRow row in dt.Rows)
            {
                var code = new Sys_ZgsCode
                {
                    CodeGroup = row["CodeGroup"]?.ToString(),
                    Code = row["Code"]?.ToString(),
                    Descname = row["Descname"]?.ToString(),
                    OrderNo = row["OrderNo"] != DBNull.Value ? Convert.ToInt32(row["OrderNo"]) : 0,
                    IsValid = row["IsValid"] != DBNull.Value ? Convert.ToString(row["IsValid"]) : "0",
                    ValidBeginDate = row["ValidBeginDate"] != DBNull.Value
                        ? Convert.ToString(row["ValidBeginDate"])
                        : null,
                    ValidEndDate = row["ValidEndDate"] != DBNull.Value ? Convert.ToString(row["ValidEndDate"]) : null,
                    ParentCode = row["ParentCode"]?.ToString(),
                    MaxPriceLimit = row["MaxPriceLimit"] != DBNull.Value
                        ? Convert.ToDecimal(row["MaxPriceLimit"])
                        : (decimal?)null
                };

                codeList.Add(code);
            }

            return codeList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢 Sys_ZgsCode 資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    
    public static OFS_SCI_Project_Main getVersionByProjectID(string ProjectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        SELECT TOP(1) *
        FROM [OFS_SCI_Project_Main]
        WHERE [ProjectID] = @ProjectID
        order by created_at DESC
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", ProjectID);

        try
        {
            DataTable dt = db.GetTable();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                var projectMain = new OFS_SCI_Project_Main();
                
                // 基本欄位
                projectMain.ID= Convert.ToInt32(row["ID"]);
                projectMain.ProjectID = row["ProjectID"]?.ToString();
                projectMain.Statuses = row["Statuses"]?.ToString();
                projectMain.StatusesName = row["StatusesName"]?.ToString();
                
                // 日期欄位 - 安全轉換
                if (row["ExpirationDate"] != DBNull.Value)
                    projectMain.ExpirationDate = Convert.ToDateTime(row["ExpirationDate"]);
                
                if (row["created_at"] != DBNull.Value)
                    projectMain.created_at = Convert.ToDateTime(row["created_at"]);
                    
                if (row["updated_at"] != DBNull.Value)
                    projectMain.updated_at = Convert.ToDateTime(row["updated_at"]);

                // 數值欄位 - 安全轉換
                if (row["SeqPoint"] != DBNull.Value)
                    projectMain.SeqPoint = Convert.ToDecimal(row["SeqPoint"]);

                // 字串欄位
                projectMain.SupervisoryUnit = row["SupervisoryUnit"]?.ToString();
                projectMain.SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString();
                projectMain.SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString();
                projectMain.UserAccount = row["UserAccount"]?.ToString();
                projectMain.UserOrg = row["UserOrg"]?.ToString();
                projectMain.UserName = row["UserName"]?.ToString();
                projectMain.Form1Status = row["Form1Status"]?.ToString();
                projectMain.Form2Status = row["Form2Status"]?.ToString();
                projectMain.Form3Status = row["Form3Status"]?.ToString();
                projectMain.Form4Status = row["Form4Status"]?.ToString();
                projectMain.Form5Status = row["Form5Status"]?.ToString();
                projectMain.CurrentStep = row["CurrentStep"]?.ToString();

                if (dt.Columns.Contains("isWithdrawal") && row["isWithdrawal"] != DBNull.Value)
                    projectMain.isWithdrawal = Convert.ToBoolean(row["isWithdrawal"]);

                if (dt.Columns.Contains("isExist") && row["isExist"] != DBNull.Value)
                    projectMain.isExist = Convert.ToBoolean(row["isExist"]);

                if (dt.Columns.Contains("ApprovedSubsidy") && row["ApprovedSubsidy"] != DBNull.Value)
                    projectMain.ApprovedSubsidy = Convert.ToDouble(row["ApprovedSubsidy"]);

                if (dt.Columns.Contains("FinalReviewNotes"))
                    projectMain.FinalReviewNotes = row["FinalReviewNotes"]?.ToString();

                if (dt.Columns.Contains("FinalReviewOrder") && row["FinalReviewOrder"] != DBNull.Value)
                    projectMain.FinalReviewOrder = Convert.ToInt32(row["FinalReviewOrder"]);

                if (dt.Columns.Contains("MidtermExamDate") && row["MidtermExamDate"] != DBNull.Value)
                    projectMain.MidtermExamDate = Convert.ToDateTime(row["MidtermExamDate"]);

                if (dt.Columns.Contains("FinalExamDate") && row["FinalExamDate"] != DBNull.Value)
                    projectMain.FinalExamDate = Convert.ToDateTime(row["FinalExamDate"]);

                if (dt.Columns.Contains("PubNumber"))
                    projectMain.PubNumber = row["PubNumber"]?.ToString();

                if (dt.Columns.Contains("ContractDate") && row["ContractDate"] != DBNull.Value)
                    projectMain.ContractDate = Convert.ToDateTime(row["ContractDate"]);
                 
                if (dt.Columns.Contains("IsProjChanged") && row["IsProjChanged"] != DBNull.Value)
                    projectMain.IsProjChanged = Convert.ToInt32(row["IsProjChanged"]);

                return projectMain;
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢 OFS_SCI_Project_Main 資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    public static void InsertOFS_SCIProjectMain(OFS_SCI_Project_Main projectMain)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
            INSERT INTO [OFS_SCI_Project_Main] (
                ProjectID,
                Statuses,
                StatusesName,
                ExpirationDate,
                SeqPoint,
                SupervisoryUnit,
                SupervisoryPersonName,
                SupervisoryPersonAccount,
                UserAccount,
                UserOrg,
                UserName,
                Form1Status,
                Form2Status,
                Form3Status,
                Form4Status,
                Form5Status,
                CurrentStep,
                created_at,
                updated_at
            )
            VALUES (
                @ProjectID,
                @Statuses,
                @StatusesName,
                @ExpirationDate,
                @SeqPoint,
                @SupervisoryUnit,
                @SupervisoryPersonName,
                @SupervisoryPersonAccount,
                @UserAccount,
                @UserOrg,
                @UserName,
                @Form1Status,
                @Form2Status,
                @Form3Status,
                @Form4Status,
                @Form5Status,
                @CurrentStep,
                @created_at,
                @updated_at
            )";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectMain.ProjectID);
            db.Parameters.Add("@Statuses", projectMain.Statuses);
            db.Parameters.Add("@StatusesName", projectMain.StatusesName);
            db.Parameters.Add("@ExpirationDate", projectMain.ExpirationDate ?? (object)DBNull.Value);
            db.Parameters.Add("@SeqPoint", projectMain.SeqPoint);
            db.Parameters.Add("@SupervisoryUnit", projectMain.SupervisoryUnit);
            db.Parameters.Add("@SupervisoryPersonName", projectMain.SupervisoryPersonName);
            db.Parameters.Add("@SupervisoryPersonAccount", projectMain.SupervisoryPersonAccount);
            db.Parameters.Add("@UserAccount", projectMain.UserAccount);
            db.Parameters.Add("@UserOrg", projectMain.UserOrg);
            db.Parameters.Add("@UserName", projectMain.UserName);
            db.Parameters.Add("@Form1Status", projectMain.Form1Status);
            db.Parameters.Add("@Form2Status", projectMain.Form2Status);
            db.Parameters.Add("@Form3Status", projectMain.Form3Status);
            db.Parameters.Add("@Form4Status", projectMain.Form4Status);
            db.Parameters.Add("@Form5Status", projectMain.Form5Status);
            db.Parameters.Add("@CurrentStep", projectMain.CurrentStep);
            db.Parameters.Add("@created_at", projectMain.created_at ?? (object)DBNull.Value);
            db.Parameters.Add("@updated_at", projectMain.updated_at ?? (object)DBNull.Value);

            try
            {
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"新增 OFS_SCI_Project_Main 資料時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static void UpdateOFS_SCIVersion(OFS_SCI_Project_Main version)
    {
        using (DbHelper db = new DbHelper())
        {
            var setClauses = new List<string>();
            db.Parameters.Clear();

            void AddIfNotNull(string column, object value)
            {
                if (value != null && !(value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    setClauses.Add($"[{column}] = @{column}");
                    db.Parameters.Add("@" + column, value);
                }
            }

            // 根據有值欄位動態組裝更新欄位
            AddIfNotNull("Statuses", version.Statuses);
            AddIfNotNull("StatusesName", version.StatusesName);
            AddIfNotNull("ExpirationDate", version.ExpirationDate);
            AddIfNotNull("QualReviewNotes", version.QualReviewNotes);
            AddIfNotNull("SeqPoint", version.SeqPoint);
            AddIfNotNull("SupervisoryUnit", version.SupervisoryUnit);
            AddIfNotNull("SupervisoryPersonName", version.SupervisoryPersonName);
            AddIfNotNull("SupervisoryPersonAccount", version.SupervisoryPersonAccount);
            AddIfNotNull("UserAccount", version.UserAccount);
            AddIfNotNull("UserOrg", version.UserOrg);
            AddIfNotNull("UserName", version.UserName);
            AddIfNotNull("Form1Status", version.Form1Status);
            AddIfNotNull("Form2Status", version.Form2Status);
            AddIfNotNull("Form3Status", version.Form3Status);
            AddIfNotNull("Form4Status", version.Form4Status);
            AddIfNotNull("Form5Status", version.Form5Status);
            AddIfNotNull("CurrentStep", version.CurrentStep);
            AddIfNotNull("created_at", version.created_at);
            AddIfNotNull("updated_at", version.updated_at);

            if (setClauses.Count == 0)
            {
                throw new Exception("沒有任何欄位需要更新。");
            }

            // 更新條件：ProjectID 為唯一鍵
            db.Parameters.Add("@ProjectID", version.ProjectID);

            db.CommandText = $@"
            UPDATE [OFS_SCI_Project_Main]
            SET {string.Join(",\n    ", setClauses)}
            WHERE [ProjectID] = @ProjectID";

            try
            {
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新 OFS_SCI_Project_Main 時發生錯誤: {ex.Message}", ex);
            }
        }
    }


    /// <summary>
    /// 取得計畫的查核標準資料 (依月份分組)
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <returns>查核標準資料按月份分組的字典</returns>
    public static Dictionary<string, List<string>> GetCheckStandardByMonth(string projectID)
    {
        var result = new Dictionary<string, List<string>>();
        
        try
        {
            using (DbHelper db = new DbHelper())
            {
                db.CommandText = @"
                    -- 先建立 CheckStandard 加上拼接欄位
                    WITH CheckStandardCTE AS (
                        SELECT 
                            *,
                            ProjectID + '_' + WorkItem AS ProjectWorkItemKey
                        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_CheckStandard]
                        WHERE ProjectID = @ProjectID
                    ),

                    -- 再建立 Main 表中：加入母項 ID
                    MainWithParentCTE AS (
                        SELECT 
                            Child.ProjectID,
                            Child.WorkItem_id AS Child_WorkItem_id,
                            Child.WorkName AS Child_WorkName,
                            Parent.WorkItem_id AS Parent_WorkItem_id,
                            Parent.WorkName AS Parent_WorkName
                        FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_Main] Child
                        LEFT JOIN [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_Main] Parent
                            ON LEFT(Child.WorkItem_id, CHARINDEX('_', Child.WorkItem_id) + 1) = Parent.WorkItem_id
                        WHERE Child.ProjectID = @ProjectID
                    )

                    -- 最終查詢：把母項對應的 CheckStandard 資料加進來
                    SELECT 
                        M.Child_WorkItem_id,
                        M.Child_WorkName,
                        M.Parent_WorkItem_id,
                        M.Parent_WorkName,
                        CS.SerialNumber,
                        CS.PlannedFinishDate,
                        CS.CheckDescription,
                        CS.SerialNumber + N'：' + CS.CheckDescription + N'（' + ISNULL(M.Parent_WorkName, N'') + N'）' AS DisplayCheckInfo

                    FROM MainWithParentCTE M
                    LEFT JOIN CheckStandardCTE CS
                        ON M.Child_WorkItem_id = CS.ProjectWorkItemKey
                    WHERE CS.PlannedFinishDate IS NOT NULL
                    ORDER BY CS.PlannedFinishDate, CS.SerialNumber";

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);

                DataTable dt = db.GetTable();
                
                foreach (DataRow row in dt.Rows)
                {
                    if (row["PlannedFinishDate"] != DBNull.Value && row["DisplayCheckInfo"] != DBNull.Value)
                    {
                        DateTime plannedDate = Convert.ToDateTime(row["PlannedFinishDate"]);
                        string displayCheckInfo = row["DisplayCheckInfo"].ToString();
                        
                        // 轉換為民國年月格式作為 Key
                        string monthKey = DateTimeHelper.ToMinguoDate(plannedDate);
                        string[] parts = monthKey.Split('/');
                        if (parts.Length >= 2)
                        {
                            monthKey = $"{parts[0]}年{int.Parse(parts[1])}月";
                        }
                        
                        // 將查核點資訊加入對應月份
                        if (!result.ContainsKey(monthKey))
                        {
                            result[monthKey] = new List<string>();
                        }
                        
                        result[monthKey].Add(displayCheckInfo);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得查核標準資料時發生錯誤: {ex.Message}");
        }
        
        return result;
    }

    /// <summary>
    /// 更新共同執行單位
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="orgPartner">共同執行單位</param>
    public static void UpdateCoExecutingUnit(string projectId, string orgPartner)
    {
        if (string.IsNullOrEmpty(projectId))
            return;
            
        DbHelper db = new DbHelper();
        
        try
        {
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
                SET [OrgPartner] = @OrgPartner,
                    [updated_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID";
            
            db.Parameters.Add("@OrgPartner", orgPartner ?? "");
            db.Parameters.Add("@ProjectID", projectId);
            
            db.ExecuteNonQuery();
            
            System.Diagnostics.Debug.WriteLine($"已更新共同執行單位: ProjectID={projectId}, OrgPartner={orgPartner}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新共同執行單位時發生錯誤: {ex.Message}");
            throw new Exception($"更新共同執行單位時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新期中期末審查日期
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="midtermExamDate">期中審查預定日期</param>
    /// <param name="finalExamDate">期末審查預定日期</param>
    public static void UpdateExamDates(string projectId, DateTime? midtermExamDate, DateTime? finalExamDate)
    {
        if (string.IsNullOrEmpty(projectId))
            return;
            
        DbHelper db = new DbHelper();
        
        try
        {
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                SET [MidtermExamDate] = @MidtermExamDate,
                    [FinalExamDate] = @FinalExamDate,
                    [updated_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID";
            
            db.Parameters.Add("@MidtermExamDate", midtermExamDate);
            db.Parameters.Add("@FinalExamDate", finalExamDate);
            db.Parameters.Add("@ProjectID", projectId);
            
            db.ExecuteNonQuery();
            
            System.Diagnostics.Debug.WriteLine($"已更新審查日期: ProjectID={projectId}, 期中={midtermExamDate?.ToString("yyyy-MM-dd")}, 期末={finalExamDate?.ToString("yyyy-MM-dd")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新審查日期時發生錯誤: {ex.Message}");
            throw new Exception($"更新審查日期時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得期中期末審查日期
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>包含期中期末審查日期的物件，若無資料則返回 null</returns>
    public static (DateTime? MidtermExamDate, DateTime? FinalExamDate) GetExamDates(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            return (null, null);
            
        DbHelper db = new DbHelper();
        
        try
        {
            db.CommandText = @"
                SELECT [MidtermExamDate], [FinalExamDate]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
                WHERE [ProjectID] = @ProjectID";
            
            db.Parameters.Add("@ProjectID", projectId);
            
            DataTable dt = db.GetTable();
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                DateTime? midtermDate = row["MidtermExamDate"] != DBNull.Value ? Convert.ToDateTime(row["MidtermExamDate"]) : (DateTime?)null;
                DateTime? finalDate = row["FinalExamDate"] != DBNull.Value ? Convert.ToDateTime(row["FinalExamDate"]) : (DateTime?)null;
                
                return (midtermDate, finalDate);
            }
            
            return (null, null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查日期時發生錯誤: {ex.Message}");
            throw new Exception($"取得審查日期時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新契約資料
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="pubNumber">發文文號</param>
    /// <param name="contractDate">簽約日期</param>
    public static void updateContractData(string projectID, string pubNumber, DateTime? contractDate)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        UPDATE [OFS_SCI_Project_Main] 
        SET 
            [PubNumber] = @PubNumber,
            [ContractDate] = @ContractDate,
            [updated_at] = @updated_at
        WHERE [ProjectID] = @ProjectID
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectID);
        db.Parameters.Add("@PubNumber", pubNumber);
        db.Parameters.Add("@ContractDate", contractDate ?? (object)DBNull.Value);
        db.Parameters.Add("@updated_at", DateTime.Now);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新契約資料失敗: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查第一期請款是否待處理
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>是否有第一期請款待處理</returns>
    public static bool IsFirstPaymentPending(string projectId)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT COUNT(*) 
            FROM [OFS_SCI_Payment] 
            WHERE [ProjectID] = @ProjectID 
            AND [Stage] = 1 
            AND [Status] = '通過'";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectId);
        
        try
        {
            var result = db.GetTable()[0][0];
            return Convert.ToInt32(result) > 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"檢查第一期請款狀態時發生錯誤: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }
    }
    
    /// <summary>
    /// 根據 ProjectID 從 OFS_SCI_Project_Main 取得申請者帳號
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>申請者帳號，未找到時返回 null</returns>
    public static string GetApplicantAccountByProjectId(string projectId)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT TOP 1 UserAccount 
            FROM OFS_SCI_Project_Main 
            WHERE ProjectID = @ProjectID 
            AND UserAccount IS NOT NULL 
            AND UserAccount != ''
            ";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", projectId);
        
        try
        {
            DataTable result = db.GetTable();
            
            if (result != null && result.Rows.Count > 0)
            {
                var userAccount = result.Rows[0]["UserAccount"];
                if (userAccount != null && userAccount != DBNull.Value)
                {
                    string accountStr = userAccount.ToString().Trim();
                    if (!string.IsNullOrEmpty(accountStr))
                    {
                        return accountStr;
                    }
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 ProjectID {projectId} 申請者帳號時發生錯誤: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新計畫的 IsProjChanged 狀態
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="isProjChanged">是否為變更狀態 (0: 否, 1: 是)</param>
    public static void UpdateIsProjChanged(string projectId, int isProjChanged)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE [OFS_SCI_Project_Main]
                SET [IsProjChanged] = @IsProjChanged,
                    [updated_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@IsProjChanged", isProjChanged);
            db.Parameters.Add("@ProjectID", projectId);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計畫 {projectId} 的 IsProjChanged 狀態時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新計畫變更完成狀態 (審查通過用)
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    public static void UpdateProjectChangeCompleted(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET IsProjChanged = 0,
                    LastOperation = @LastOperation,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectID";
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@LastOperation", "已完成計畫變更");
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計畫變更完成狀態時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新最新一筆計畫變更記錄狀態
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="status">狀態 (1: 變更中, 3: 已完成)</param>
    /// <param name="rejectReason">退回原因 (通過時傳空字串)</param>
    public static void UpdateProjectChangeRecordStatus(string projectId, int status, string rejectReason ="")
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE OFS_ProjectChangeRecord
                SET Status = @Status,
                    RejectReason = @RejectReason,
                    UpdateTime = GETDATE()
                WHERE DataID = @ProjectID
                  AND Method = 1
                  AND Type = 'SCI'
                  AND ID = (
                      SELECT TOP 1 ID
                      FROM OFS_ProjectChangeRecord
                      WHERE DataID = @ProjectID AND Method = 1 AND Type = 'SCI'
                      ORDER BY CreateTime DESC
                  )";
            db.Parameters.Add("@Status", status);
            db.Parameters.Add("@RejectReason", rejectReason);
            db.Parameters.Add("@ProjectID", projectId);
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計畫變更記錄狀態時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 刪除預定進度資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    public static void DeletePreMonthProgress(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = "DELETE FROM OFS_SCI_PreMonthProgress WHERE ProjectID = @ProjectID";
            db.Parameters.Add("@ProjectID", projectId);
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"刪除預定進度資料時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新計畫變更為退回狀態 (審查不通過用)
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    public static void UpdateProjectChangeRejected(string projectId)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET IsProjChanged = 1,
                    updated_at = GETDATE()
                WHERE ProjectID = @ProjectID";
            db.Parameters.Add("@ProjectID", projectId);
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計畫變更為退回狀態時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新計畫的追回金額並設定為結案(未通過)
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="recoveryAmount">追回金額</param>
    public static void UpdateRecoveryAmount(string projectID, decimal recoveryAmount)
    {
        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                UPDATE [OFS_SCI_Project_Main]
                SET [RecoveryAmount] = @RecoveryAmount,
                    [StatusesName] = @StatusesName,
                    [updated_at] = GETDATE()
                WHERE [ProjectID] = @ProjectID
            ";

            db.Parameters.Add("@ProjectID", projectID);
            db.Parameters.Add("@RecoveryAmount", recoveryAmount);
            db.Parameters.Add("@StatusesName", "已終止");

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新追回金額時發生錯誤: {ex.Message}", ex);
        }
        finally
        {
            db.Parameters.Clear();
            db.Dispose();
        }
    }

}