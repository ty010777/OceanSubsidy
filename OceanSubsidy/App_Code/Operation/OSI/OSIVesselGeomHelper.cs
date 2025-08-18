using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIVesselGeomHelper 的摘要描述
/// </summary>
public class OSIVesselGeomHelper
{
    public OSIVesselGeomHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 查詢所有
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [GeomID]
    ,[AssessmentId]
    ,[GeomName]
    ,[GeoData]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
WHERE IsValid = 1
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By ID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByID(string id)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [GeomID]
    ,[AssessmentId]
    ,[GeomName]
    ,[GeoData]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
WHERE IsValid = 1
AND GeomID = @GeomID
";
        db.Parameters.Clear();
        db.Parameters.Add("@GeomID", id);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By AssessmentId
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <returns></returns>
    public static GisTable QueryByAssessmentId(int assessmentId)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [GeomID]
    ,[AssessmentId]
    ,[GeomName]
    ,[GeoData].STAsText() AS GeoData
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
WHERE IsValid = 1 AND DeletedAt IS NULL
AND AssessmentId = @AssessmentId
ORDER BY CreatedAt
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢地圖資訊 BY AssessmentId
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <returns></returns>
    public static string QueryGeoDataByAssessmentId(int assessmentId)
    {
        string rtVal = "";
        
        var geomsTable = QueryByAssessmentId(assessmentId);
        if (geomsTable != null && geomsTable.Rows.Count > 0)
        {
            if (geomsTable.Rows.Count == 1)
            {
                // 單一圖徵
                rtVal = geomsTable.Rows[0]["GeoData"].ToString();
            }
            else
            {
                // 多個圖徵，組合成 GeometryCollection
                var geoms = new List<string>();
                foreach (System.Data.DataRow row in geomsTable.Rows)
                {
                    if (row["GeoData"] != null && row["GeoData"] != DBNull.Value)
                    {
                        string geom = row["GeoData"].ToString();
                        if (!string.IsNullOrEmpty(geom))
                        {
                            geoms.Add(geom);
                        }
                    }
                }
                
                if (geoms.Count > 0)
                {
                    rtVal = $"GEOMETRYCOLLECTION({string.Join(",", geoms)})";
                }
            }
        }
        
        return rtVal;
    }

    /// <summary>
    /// 軟刪除特定 AssessmentId 的所有地理資料
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <param name="deletedBy"></param>
    /// <returns></returns>
    public static bool SoftDeleteByAssessmentId(int assessmentId, int deletedBy)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
SET IsValid = 0, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
WHERE AssessmentId = @AssessmentId AND IsValid = 1 AND DeletedAt IS NULL
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);
        db.Parameters.Add("@DeletedBy", deletedBy);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 新增地理資料
    /// </summary>
    /// <param name="geom"></param>
    /// <returns></returns>
    public static bool Insert(OSI_VesselGeom geom)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
    ([GeomID], [AssessmentId], [GeomName], [GeoData], [IsValid], [CreatedAt])
VALUES
    (@GeomID, @AssessmentId, @GeomName, geometry::STGeomFromText(@GeoData, 3826), @IsValid, @CreatedAt)
";
        db.Parameters.Clear();
        db.Parameters.Add("@GeomID", geom.GeomID);
        db.Parameters.Add("@AssessmentId", geom.AssessmentId);
        db.Parameters.Add("@GeomName", geom.GeomName);
        db.Parameters.Add("@GeoData", geom.GeoData);
        db.Parameters.Add("@IsValid", geom.IsValid);
        db.Parameters.Add("@CreatedAt", geom.CreatedAt);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 儲存地理資料（支援 JSON 格式和智慧更新）
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <param name="wktData"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static bool SaveGeometries(int assessmentId, string wktData, int userId)
    {
        bool success = false;
        DbHelper db = new DbHelper();
        
        db.BeginTrans();
        try
        {
            // 如果是空的或 null，則軟刪除所有現有圖徵
            if (string.IsNullOrEmpty(wktData) || string.Equals(wktData, "null", StringComparison.OrdinalIgnoreCase))
            {
                db.CommandText = @"
UPDATE OSI_VesselGeom 
SET IsValid = 0, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
WHERE AssessmentId = @AssessmentId AND IsValid = 1 AND DeletedAt IS NULL";
                
                db.Parameters.Clear();
                db.Parameters.Add("@AssessmentId", assessmentId);
                db.Parameters.Add("@DeletedBy", userId);
                db.ExecuteNonQuery();
            }
            else
            {
                // 嘗試解析 JSON 格式
                try
                {
                    var featureCollection = Newtonsoft.Json.JsonConvert.DeserializeObject<FeatureCollection>(wktData);
                    if (featureCollection != null && featureCollection.features != null)
                    {
                        // 取得現有的圖徵
                        var existingGeoms = QueryByAssessmentIdWithClass(assessmentId);
                        var existingDict = existingGeoms.ToDictionary(g => g.GeomID, g => g);
                        var processedIds = new HashSet<string>();

                        // 處理每個 feature
                        foreach (var feature in featureCollection.features)
                        {
                            if (!string.IsNullOrEmpty(feature.id) && existingDict.ContainsKey(feature.id))
                            {
                                // 檢查現有圖徵是否有變更
                                var existingGeom = existingDict[feature.id];
                                var newName = feature.name ?? "";
                                var newWkt = feature.wkt ?? "";
                                
                                // 判斷 GeomName 或 WKT 是否有變更
                                bool hasNameChanges = !string.Equals(existingGeom.GeomName, newName, StringComparison.Ordinal);
                                
                                // 比較 WKT 到小數第4位
                                bool hasWktChanges = false;
                                if (!string.IsNullOrEmpty(existingGeom.GeoData) && !string.IsNullOrEmpty(newWkt))
                                {
                                    // 正規化 WKT 字串到小數第4位來比較
                                    var normalizedExisting = NormalizeWktPrecision(existingGeom.GeoData, 4);
                                    var normalizedNew = NormalizeWktPrecision(newWkt, 4);
                                    hasWktChanges = !string.Equals(normalizedExisting, normalizedNew, StringComparison.Ordinal);
                                }
                                else if (string.IsNullOrEmpty(existingGeom.GeoData) != string.IsNullOrEmpty(newWkt))
                                {
                                    // 一個是空的，另一個不是
                                    hasWktChanges = true;
                                }
                                
                                bool hasChanges = hasNameChanges || hasWktChanges;
                                
                                if (hasChanges)
                                {
                                    // 有變更：軟刪除舊資料
                                    SoftDeleteByGeomId(existingGeom.GeomID, userId);
                                    
                                    // 新增一筆新資料
                                    var newGeom = new OSI_VesselGeom
                                    {
                                        GeomID = Guid.NewGuid().ToString().ToUpper(),
                                        AssessmentId = assessmentId,
                                        GeomName = newName,
                                        GeoData = newWkt,
                                        IsValid = true,
                                        CreatedAt = DateTime.Now
                                    };
                                    InsertWithGeometry(newGeom);
                                }
                                
                                processedIds.Add(feature.id);
                            }
                            else
                            {
                                // 新增圖徵
                                var newGeom = new OSI_VesselGeom
                                {
                                    GeomID = Guid.NewGuid().ToString().ToUpper(),
                                    AssessmentId = assessmentId,
                                    GeomName = feature.name ?? "研究船風險檢核範圍",
                                    GeoData = feature.wkt,
                                    IsValid = true,
                                    CreatedAt = DateTime.Now
                                };
                                InsertWithGeometry(newGeom);
                            }
                        }

                        // 軟刪除未處理的圖徵
                        foreach (var existingGeom in existingGeoms)
                        {
                            if (!processedIds.Contains(existingGeom.GeomID))
                            {
                                SoftDeleteByGeomId(existingGeom.GeomID, userId);
                            }
                        }
                    }
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    // JSON 解析失敗，嘗試當作單一 WKT 處理
                    db.CommandText = @"
UPDATE OSI_VesselGeom 
SET IsValid = 0, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
WHERE AssessmentId = @AssessmentId AND IsValid = 1 AND DeletedAt IS NULL";
                    
                    db.Parameters.Clear();
                    db.Parameters.Add("@AssessmentId", assessmentId);
                    db.Parameters.Add("@DeletedBy", userId);
                    db.ExecuteNonQuery();

                    // 新增單一圖徵
                    var newGeom = new OSI_VesselGeom
                    {
                        GeomID = Guid.NewGuid().ToString().ToUpper(),
                        AssessmentId = assessmentId,
                        GeomName = "研究船風險檢核範圍",
                        GeoData = wktData,
                        IsValid = true,
                        CreatedAt = DateTime.Now
                    };
                    InsertWithGeometry(newGeom);
                }
            }
            
            db.Commit();
            success = true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            throw new Exception($"儲存地理資料失敗：{ex.Message}");
        }
        
        return success;
    }

    /// <summary>
    /// 查詢 By AssessmentId（返回類別清單）
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <returns></returns>
    public static List<OSI_VesselGeom> QueryByAssessmentIdWithClass(int assessmentId)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [GeomID]
    ,[AssessmentId]
    ,[GeomName]
    ,[GeoData].STAsText() AS GeoData
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
WHERE IsValid = 1 AND DeletedAt IS NULL
AND AssessmentId = @AssessmentId
ORDER BY CreatedAt";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);

        return db.GetList<OSI_VesselGeom>();
    }

    /// <summary>
    /// 軟刪除特定 GeomID 的地理資料
    /// </summary>
    /// <param name="geomId"></param>
    /// <param name="deletedBy"></param>
    /// <returns></returns>
    public static bool SoftDeleteByGeomId(string geomId, int deletedBy)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
SET IsValid = 0, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
WHERE GeomID = @GeomID AND IsValid = 1 AND DeletedAt IS NULL";
        db.Parameters.Clear();
        db.Parameters.Add("@GeomID", geomId);
        db.Parameters.Add("@DeletedBy", deletedBy);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 新增地理資料（使用 geometry::STGeomFromText）
    /// </summary>
    /// <param name="geom"></param>
    /// <returns></returns>
    public static bool InsertWithGeometry(OSI_VesselGeom geom)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselGeom]
    ([GeomID], [AssessmentId], [GeomName], [GeoData], [IsValid], [CreatedAt])
VALUES
    (@GeomID, @AssessmentId, @GeomName, geometry::STGeomFromText(@GeoData, 3826), @IsValid, @CreatedAt)";
        db.Parameters.Clear();
        db.Parameters.Add("@GeomID", geom.GeomID);
        db.Parameters.Add("@AssessmentId", geom.AssessmentId);
        db.Parameters.Add("@GeomName", geom.GeomName);
        db.Parameters.Add("@GeoData", geom.GeoData);
        db.Parameters.Add("@IsValid", geom.IsValid);
        db.Parameters.Add("@CreatedAt", geom.CreatedAt);

        try
        {
            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 將 WKT 字串中的座標正規化到指定的小數位數，並標準化格式
    /// </summary>
    /// <param name="wkt">原始 WKT 字串</param>
    /// <param name="precision">小數位數</param>
    /// <returns>正規化後的 WKT 字串</returns>
    private static string NormalizeWktPrecision(string wkt, int precision)
    {
        if (string.IsNullOrEmpty(wkt))
            return wkt;

        // 先正規化座標精度
        var pattern = @"-?\d+\.?\d*";
        var result = System.Text.RegularExpressions.Regex.Replace(wkt, pattern, match =>
        {
            if (double.TryParse(match.Value, out double value))
            {
                return Math.Round(value, precision).ToString($"F{precision}");
            }
            return match.Value;
        });

        // 標準化 WKT 格式：移除幾何類型和括號之間的空格
        // 例如：POINT (x y) -> POINT(x y)
        result = System.Text.RegularExpressions.Regex.Replace(result, @"(POINT|LINESTRING|POLYGON|MULTIPOINT|MULTILINESTRING|MULTIPOLYGON|GEOMETRYCOLLECTION)\s+\(", "$1(");
        
        // 標準化座標之間的空格：確保只有一個空格
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");
        
        // 移除括號內部開頭和結尾的空格
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\(\s+", "(");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+\)", ")");
        
        // 標準化逗號後的空格：確保逗號後有一個空格
        result = System.Text.RegularExpressions.Regex.Replace(result, @",\s*", ", ");

        return result.Trim();
    }

    /// <summary>
    /// Feature Collection 類別，用於解析前端傳來的 JSON 資料
    /// </summary>
    private class FeatureCollection
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }

    /// <summary>
    /// Feature 類別
    /// </summary>
    private class Feature
    {
        public string id { get; set; }
        public string name { get; set; }
        public string wkt { get; set; }
    }

}