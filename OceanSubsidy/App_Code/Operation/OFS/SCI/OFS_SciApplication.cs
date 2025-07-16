using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
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
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }


    public static void insertApplicationMain(OFS_SCI_Application_Main applicationData)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main]
(
    [ProjectID],
    [PersonID],
    [KeywordID],
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
    [RegisteredAddress],
    [CorrespondenceAddress],
    [Target],
    [Summary],
    [Innovation],
    [Declaration]

)
VALUES
(
    @ProjectID,              -- 計畫編號 (年度 + 流水號4碼)
    @PersonID,               -- 人員申請表外鍵
    @KeywordID,              -- 關鍵字外鍵
    @Year,                   -- 年度
    @Serial,
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
    @RegisteredAddress,      -- 登記地址
    @CorrespondenceAddress,  -- 通訊地址
    @Target,                 -- 計畫目標
    @Summary,                -- 計畫內容摘要
    @Innovation,             -- 創新重點
    @Declaration            -- 聲明同意書


)";

        db.Parameters.Clear();
        db.Parameters.Add("@ProjectID", applicationData.ProjectID);
        db.Parameters.Add("@PersonID", applicationData.PersonID);
        db.Parameters.Add("@KeywordID", applicationData.KeywordID);
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
        db.Parameters.Add("@RegisteredAddress", applicationData.RegisteredAddress);
        db.Parameters.Add("@CorrespondenceAddress", applicationData.CorrespondenceAddress);
        db.Parameters.Add("@Target", applicationData.Target);
        db.Parameters.Add("@Summary", applicationData.Summary);
        db.Parameters.Add("@Innovation", applicationData.Innovation);
        db.Parameters.Add("@Declaration", applicationData.Declaration);

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
            AddIfNotNull("KeywordID", applicationData.KeywordID);
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
            AddIfNotNull("RegisteredAddress", applicationData.RegisteredAddress);
            AddIfNotNull("CorrespondenceAddress", applicationData.CorrespondenceAddress);
            AddIfNotNull("Target", applicationData.Target);
            AddIfNotNull("Summary", applicationData.Summary);
            AddIfNotNull("Innovation", applicationData.Innovation);
            AddIfNotNull("Declaration", applicationData.Declaration);

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
                    KeywordID = row["KeywordID"]?.ToString(),
                    Year = row["Year"] != DBNull.Value ? Convert.ToInt32(row["Year"]) : 0,
                    Serial = row["Serial"]?.ToString(),
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
                    RegisteredAddress = row["RegisteredAddress"]?.ToString(),
                    CorrespondenceAddress = row["CorrespondenceAddress"]?.ToString(),
                    Target = row["Target"]?.ToString(),
                    Summary = row["Summary"]?.ToString(),
                    Innovation = row["Innovation"]?.ToString(),
                    Declaration = row["Declaration"] != DBNull.Value ? (bool?)row["Declaration"] : null,
                    created_at = row["created_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["created_at"])
                        : (DateTime?)null,
                    updated_at = row["updated_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["updated_at"])
                        : (DateTime?)null,
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

    public static Tuple<string, string, string> GetProjectPersonKeyword(string ProjectID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"SELECT [ProjectID], [PersonID], [KeywordID]
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
                    row["PersonID"]?.ToString(),
                    row["KeywordID"]?.ToString()
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
                    KeywordID = row["KeywordID"]?.ToString(),
                    Year = row["Year"] != DBNull.Value ? Convert.ToInt32(row["Year"]) : 0,
                    Serial = row["Serial"]?.ToString(),
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
                    RegisteredAddress = row["RegisteredAddress"]?.ToString(),
                    CorrespondenceAddress = row["CorrespondenceAddress"]?.ToString(),
                    Target = row["Target"]?.ToString(),
                    Summary = row["Summary"]?.ToString(),
                    Innovation = row["Innovation"]?.ToString(),
                    Declaration = row["Declaration"] != DBNull.Value ? (bool?)row["Declaration"] : null,
                    created_at = row["created_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["created_at"])
                        : (DateTime?)null,
                    updated_at = row["updated_at"] != DBNull.Value
                        ? Convert.ToDateTime(row["updated_at"])
                        : (DateTime?)null
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
        WHERE CodeGroup = @CodeGroup
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
                return new OFS_SCI_Project_Main
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Statuses = row["Statuses"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                    SeqPoint = row["SeqPoint"] != DBNull.Value ? Convert.ToDecimal(row["SeqPoint"]) : 0,
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    UserAccount = row["UserAccount"]?.ToString(),
                    UserOrg = row["UserOrg"]?.ToString(),
                    UserName = row["UserName"]?.ToString(),
                    Form1Status = row["Form1Status"]?.ToString(),
                    Form2Status = row["Form2Status"]?.ToString(),
                    Form3Status = row["Form3Status"]?.ToString(),
                    Form4Status = row["Form4Status"]?.ToString(),
                    Form5Status = row["Form5Status"]?.ToString(),
                    Form6Status = row["Form6Status"]?.ToString(),
                    CurrentStep = row["CurrentStep"]?.ToString(),
                    created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,
                    updated_at = row["updated_at"] != DBNull.Value ? (DateTime?)row["updated_at"] : null
                };
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

    public static void InsertOFS_SCIVersion(OFS_SCI_Project_Main version)
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
                Form6Status,
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
                @Form6Status,
                @CurrentStep,
                @created_at,
                @updated_at
            )";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", version.ProjectID);
            db.Parameters.Add("@Statuses", version.Statuses);
            db.Parameters.Add("@StatusesName", version.StatusesName);
            db.Parameters.Add("@ExpirationDate", version.ExpirationDate ?? (object)DBNull.Value);
            db.Parameters.Add("@SeqPoint", version.SeqPoint);
            db.Parameters.Add("@SupervisoryUnit", version.SupervisoryUnit);
            db.Parameters.Add("@SupervisoryPersonName", version.SupervisoryPersonName);
            db.Parameters.Add("@SupervisoryPersonAccount", version.SupervisoryPersonAccount);
            db.Parameters.Add("@UserAccount", version.UserAccount);
            db.Parameters.Add("@UserOrg", version.UserOrg);
            db.Parameters.Add("@UserName", version.UserName);
            db.Parameters.Add("@Form1Status", version.Form1Status);
            db.Parameters.Add("@Form2Status", version.Form2Status);
            db.Parameters.Add("@Form3Status", version.Form3Status);
            db.Parameters.Add("@Form4Status", version.Form4Status);
            db.Parameters.Add("@Form5Status", version.Form5Status);
            db.Parameters.Add("@Form6Status", version.Form6Status);
            db.Parameters.Add("@CurrentStep", version.CurrentStep);
            db.Parameters.Add("@created_at", version.created_at ?? (object)DBNull.Value);
            db.Parameters.Add("@updated_at", version.updated_at ?? (object)DBNull.Value);

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
            AddIfNotNull("Form6Status", version.Form6Status);
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
}