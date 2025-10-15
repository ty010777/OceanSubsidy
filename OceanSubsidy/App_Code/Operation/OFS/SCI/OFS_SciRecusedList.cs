using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class OFS_SciRecusedList : System.Web.UI.Page
{
    public OFS_SciRecusedList()
    {
       
    }
        public static void ReplaceRecusedList(List<OFS_SCI_Other_Recused> recusedList, string ProjectID, bool chkNoAvoidance)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 1. 先刪除該 ProjectID 的資料
                    db.CommandText = @"
                        DELETE FROM OFS_SCI_Other_Recused
                        WHERE ProjectID = @ProjectID";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
                    db.ExecuteNonQuery();
                    if(chkNoAvoidance == true)
                    {
                        // 如果選擇了「無迴避名單」，則不插入任何資料
                        return;
                    }
                    // 2. 再逐筆插入新的資料
                    foreach (var recused in recusedList)
                    {
                        db.CommandText = @"
                            INSERT INTO OFS_SCI_Other_Recused
                            (ProjectID, RecusedName, EmploymentUnit, JobTitle, RecusedReason)
                            VALUES
                            (@ProjectID, @RecusedName, @EmploymentUnit, @JobTitle, @RecusedReason)";
                        db.Parameters.Clear();
                        db.Parameters.Add("@ProjectID", ProjectID);
                        db.Parameters.Add("@RecusedName", recused.RecusedName);
                        db.Parameters.Add("@EmploymentUnit", recused.EmploymentUnit);
                        db.Parameters.Add("@JobTitle", recused.JobTitle);
                        db.Parameters.Add("@RecusedReason", recused.RecusedReason);

                        db.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"更新迴避名單資料時發生錯誤: {ex.Message}", ex);
                }
            }
        }
        public static void UpdateIsRecused( string ProjectID, bool chkNoAvoidance)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    db.CommandText = @"
                            update OFS_SCI_Application_Main
                            set IsRecused = @chkNoAvoidance
                            where ProjectID = @ProjectID";
                    db.Parameters.Add("@ProjectID", ProjectID);
                    db.Parameters.Add("@chkNoAvoidance", chkNoAvoidance);

                    db.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    throw new Exception($"更新迴避名單資料時發生錯誤: {ex.Message}", ex);

                }
            }


        }
        public static void ReplaceTechReadinessList(List<OFS_SCI_Other_TechReadiness> techList, string ProjectID)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 1. 刪除該 ProjectID 的資料
                    db.CommandText = @"
                DELETE FROM OFS_SCI_Other_TechReadiness
                WHERE ProjectID = @ProjectID";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
                    db.ExecuteNonQuery();

                    // 2. 逐筆插入新資料
                    foreach (var tech in techList)
                    {
                        db.CommandText = @"
                    INSERT INTO OFS_SCI_Other_TechReadiness
                    (ProjectID, Name, Bef_TRLevel, Aft_TRLevel, Description)
                    VALUES
                    (@ProjectID, @Name, @Bef_TRLevel, @Aft_TRLevel, @Description)";
                        db.Parameters.Clear();
                        db.Parameters.Add("@ProjectID", ProjectID);
                        db.Parameters.Add("@Name", tech.Name);
                        db.Parameters.Add("@Bef_TRLevel", tech.Bef_TRLevel);
                        db.Parameters.Add("@Aft_TRLevel", tech.Aft_TRLevel);
                        db.Parameters.Add("@Description", tech.Description);

                        db.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"更新技術成熟度資料時發生錯誤: {ex.Message}", ex);
                }
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
                                ValidBeginDate = row["ValidBeginDate"] != DBNull.Value ? Convert.ToString(row["ValidBeginDate"]) :null,
                                ValidEndDate = row["ValidEndDate"] != DBNull.Value ? Convert.ToString(row["ValidEndDate"]) : null,
                                ParentCode = row["ParentCode"]?.ToString(),
                                MaxPriceLimit = row["MaxPriceLimit"] != DBNull.Value ? Convert.ToDecimal(row["MaxPriceLimit"]) : 0
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
                public static List<OFS_SCI_Other_Recused> GetRecusedListByProjectID(string ProjectID)
                {
                    DbHelper db = new DbHelper();
                    db.CommandText = @"
                        SELECT *
                        FROM OFS_SCI_Other_Recused
                        WHERE ProjectID = @ProjectID
                        ORDER BY RecusedName";

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);

                    try
                    {
                        DataTable dt = db.GetTable();
                        List<OFS_SCI_Other_Recused> list = new List<OFS_SCI_Other_Recused>();

                        foreach (DataRow row in dt.Rows)
                        {
                            var item = new OFS_SCI_Other_Recused
                            {
                                ProjectID = row["ProjectID"]?.ToString(),
                                RecusedName = row["RecusedName"]?.ToString(),
                                EmploymentUnit = row["EmploymentUnit"]?.ToString(),
                                JobTitle = row["JobTitle"]?.ToString(),
                                RecusedReason = row["RecusedReason"]?.ToString()
                            };

                            list.Add(item);
                        }

                        return list;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"查詢 Recused 資料時發生錯誤: {ex.Message}", ex);
                    }
                }
                public static List<OFS_SCI_Other_TechReadiness> GetTechReadinessListByProjectID(string ProjectID)
                {
                    DbHelper db = new DbHelper();
                    db.CommandText = @"
                        SELECT *
                        FROM OFS_SCI_Other_TechReadiness
                        WHERE ProjectID = @ProjectID
                        ORDER BY Name";

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);

                    try
                    {
                        DataTable dt = db.GetTable();
                        List<OFS_SCI_Other_TechReadiness> list = new List<OFS_SCI_Other_TechReadiness>();

                        foreach (DataRow row in dt.Rows)
                        {
                            var item = new OFS_SCI_Other_TechReadiness
                            {
                                ProjectID = row["ProjectID"]?.ToString(),
                                Name = row["Name"]?.ToString(),
                                Bef_TRLevel = row["Bef_TRLevel"]?.ToString(),
                                Aft_TRLevel = row["Aft_TRLevel"]?.ToString(),
                                Description = row["Description"]?.ToString()
                            };

                            list.Add(item);
                        }

                        return list;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"查詢技術成熟度資料時發生錯誤: {ex.Message}", ex);
                    }
                }

    /// <summary>
    /// 更新 Form5Status
    /// </summary>
    /// <param name="projectId">ProjectID</param>
    /// <param name="status">狀態 (暫存 或 完成)</param>
    public static void UpdateForm4Status(string projectId, string status)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET Form5Status = @Status
                WHERE ProjectID = @ProjectId";

                db.Parameters.Clear();
                db.Parameters.Add("@Status", status);
                db.Parameters.Add("@ProjectId", projectId);

                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新 Form4Status 時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 更新 Form5Status 和 CurrentStep
    /// </summary>
    /// <param name="projectId">ProjectID</param>
    /// <param name="status">狀態 (暫存 或 完成)</param>
    /// <param name="currentStep">當前步驟</param>
    public static void UpdateForm4StatusAndCurrentStep(string projectId, string status, string currentStep)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET Form4Status = @Status, CurrentStep = @CurrentStep
                WHERE ProjectID = @ProjectId";

                db.Parameters.Clear();
                db.Parameters.Add("@Status", status);
                db.Parameters.Add("@CurrentStep", currentStep);
                db.Parameters.Add("@ProjectId", projectId);

                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新 Form4Status 和 CurrentStep 時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 取得專案的 IsRecused 狀態
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>IsRecused 狀態</returns>
    public static bool GetIsRecusedByProjectID(string projectID)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                    SELECT ISNULL(IsRecused, 0) as IsRecused
                    FROM OFS_SCI_Application_Main
                    WHERE ProjectID = @ProjectID";

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectID);

                var result = db.GetTable().Rows[0][0] ;
                return Convert.ToBoolean(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"取得 IsRecused 狀態時發生錯誤: {ex.Message}", ex);
            }
        }
    }

    public static List<SCI_OtherRecused> queryRecusedList(int year, string keyword, string name, string org)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                SELECT B.[Year]
                      ,A.[ProjectID]
                      ,B.[ProjectNameTw]
                      ,B.[OrgName]
                      ,A.[RecusedName]
                      ,A.[EmploymentUnit]
                      ,A.[JobTitle]
                      ,A.[RecusedReason]
                  FROM [OFS_SCI_Other_Recused] AS A
                  JOIN [OFS_SCI_Application_Main] AS B ON B.[ProjectID] = A.[ProjectID]
                 WHERE 1 = 1
            ";

            if (year > 0)
            {
                db.CommandText += " AND B.Year = @Year";
                db.Parameters.Add("@Year", year);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                db.CommandText += " AND (A.RecusedName LIKE @Keyword OR A.EmploymentUnit LIKE @Keyword OR A.JobTitle LIKE @Keyword)";
                db.Parameters.Add("@Keyword", "%" + keyword.Trim() + "%");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                db.CommandText += " AND B.ProjectNameTw LIKE @Name";
                db.Parameters.Add("@Name", "%" + name.Trim() + "%");
            }

            if (!string.IsNullOrWhiteSpace(org))
            {
                db.CommandText += " AND B.OrgName LIKE @Org";
                db.Parameters.Add("@Org", "%" + org.Trim() + "%");
            }

            return db.GetTable().Rows.Cast<DataRow>().Select(row => new SCI_OtherRecused {
                Year = row.Field<int>("Year"),
                ProjectID = row.Field<string>("ProjectID"),
                ProjectNameTw = row.Field<string>("ProjectNameTw"),
                OrgName = row.Field<string>("OrgName"),
                RecusedName = row.Field<string>("RecusedName"),
                EmploymentUnit = row.Field<string>("EmploymentUnit"),
                JobTitle = row.Field<string>("JobTitle"),
                RecusedReason = row.Field<string>("RecusedReason")
            }).ToList();
        }
    }
}
