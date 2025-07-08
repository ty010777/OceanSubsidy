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
public class OFS_SciRecusedList : System.Web.UI.Page
{
    public OFS_SciRecusedList()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }   
        public static void ReplaceRecusedList(List<OFS_SCI_Other_Recused> recusedList, string Version_ID)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 1. 先刪除該 Version_ID 的資料
                    db.CommandText = @"
                        DELETE FROM OFS_SCI_Other_Recused
                        WHERE Version_ID = @Version_ID";
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.ExecuteNonQuery();
        
                    // 2. 再逐筆插入新的資料
                    foreach (var recused in recusedList)
                    {
                        db.CommandText = @"
                            INSERT INTO OFS_SCI_Other_Recused
                            (Version_ID, RecusedName, EmploymentUnit, JobTitle, RecusedReason)
                            VALUES
                            (@Version_ID, @RecusedName, @EmploymentUnit, @JobTitle, @RecusedReason)";
                        db.Parameters.Clear();
                        db.Parameters.Add("@Version_ID", Version_ID);
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
        public static void ReplaceTechReadinessList(List<OFS_SCI_Other_TechReadiness> techList, string Version_ID)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 1. 刪除該 Version_ID 的資料
                    db.CommandText = @"
                DELETE FROM OFS_SCI_Other_TechReadiness
                WHERE Version_ID = @Version_ID";
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);
                    db.ExecuteNonQuery();

                    // 2. 逐筆插入新資料
                    foreach (var tech in techList)
                    {
                        db.CommandText = @"
                    INSERT INTO OFS_SCI_Other_TechReadiness
                    (Version_ID, Name, Bef_TRLevel, Aft_TRLevel, Description)
                    VALUES
                    (@Version_ID, @Name, @Bef_TRLevel, @Aft_TRLevel, @Description)";
                        db.Parameters.Clear();
                        db.Parameters.Add("@Version_ID", Version_ID);
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
                public static List<OFS_SCI_Other_Recused> GetRecusedListByVersion_ID(string Version_ID)
                {
                    DbHelper db = new DbHelper();
                    db.CommandText = @"
                        SELECT *
                        FROM OFS_SCI_Other_Recused
                        WHERE Version_ID = @Version_ID
                        ORDER BY RecusedName";

                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);

                    try
                    {
                        DataTable dt = db.GetTable();
                        List<OFS_SCI_Other_Recused> list = new List<OFS_SCI_Other_Recused>();

                        foreach (DataRow row in dt.Rows)
                        {
                            var item = new OFS_SCI_Other_Recused
                            {
                                Version_ID = row["Version_ID"]?.ToString(),
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
                public static List<OFS_SCI_Other_TechReadiness> GetTechReadinessListByVersion_ID(string Version_ID)
                {
                    DbHelper db = new DbHelper();
                    db.CommandText = @"
                        SELECT *
                        FROM OFS_SCI_Other_TechReadiness
                        WHERE Version_ID = @Version_ID
                        ORDER BY Name";

                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", Version_ID);

                    try
                    {
                        DataTable dt = db.GetTable();
                        List<OFS_SCI_Other_TechReadiness> list = new List<OFS_SCI_Other_TechReadiness>();

                        foreach (DataRow row in dt.Rows)
                        {
                            var item = new OFS_SCI_Other_TechReadiness
                            {
                                Version_ID = row["Version_ID"]?.ToString(),
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


}