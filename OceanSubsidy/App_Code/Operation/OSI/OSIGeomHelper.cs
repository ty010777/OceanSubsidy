using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIGeomHelper 的摘要描述
/// </summary>
public class OSIGeomHelper
{
    public OSIGeomHelper()
    {
        
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
    ,[ReportID]
    ,[GeomName]
    ,[GeoData]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_Geom]
WHERE IsValid = 1";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 依據 ReportID 查詢圖徵
    /// </summary>
    public static GisTable QueryByReportID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [GeomID]
    ,[ReportID]
    ,[GeomName]
    ,[GeoData].STAsText() AS GeoData
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_Geom]
WHERE ReportID = @ReportID 
    AND IsValid = 1 
    AND DeletedAt IS NULL
ORDER BY GeomID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }

    /// <summary>
    /// 依據 ReportID 查詢圖徵（返回類別清單）
    /// </summary>
    public static List<OSI_Geom> QueryByReportIDWithClass(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [GeomID]
    ,[ReportID]
    ,[GeomName]
    ,[GeoData]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_Geom]
WHERE ReportID = @ReportID 
    AND IsValid = 1 
    AND DeletedAt IS NULL
ORDER BY GeomID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetList<OSI_Geom>();
    }

    /// <summary>
    /// 新增圖徵
    /// </summary>
    public static bool InsertGeom(OSI_Geom geom)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
INSERT INTO [dbo].[OSI_Geom]
    ([GeomID]
    ,[ReportID]
    ,[GeomName]
    ,[GeoData]
    ,[IsValid]
    ,[CreatedAt])
VALUES
    (@GeomID
    ,@ReportID
    ,@GeomName
    ,geometry::STGeomFromText(@GeoData, 3826)
    ,@IsValid
    ,@CreatedAt)";

            db.Parameters.Clear();
            db.Parameters.Add("@GeomID", string.IsNullOrEmpty(geom.GeomID) ? Guid.NewGuid().ToString().ToUpper() : geom.GeomID);
            db.Parameters.Add("@ReportID", geom.ReportID);
            db.Parameters.Add("@GeomName", geom.GeomName);
            db.Parameters.Add("@GeoData", geom.GeoData);
            db.Parameters.Add("@IsValid", geom.IsValid);
            db.Parameters.Add("@CreatedAt", DateTime.Now);

            db.ExecuteNonQuery();
            rtVal = true;
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 刪除圖徵（軟刪除）
    /// </summary>
    public static bool DeleteGeom(string geomID, int deletedBy)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
UPDATE [dbo].[OSI_Geom]
SET [IsValid] = 0
    ,[DeletedAt] = GETDATE()
    ,[DeletedBy] = @DeletedBy
WHERE GeomID = @GeomID";

            db.Parameters.Clear();
            db.Parameters.Add("@GeomID", geomID);
            db.Parameters.Add("@DeletedBy", deletedBy);

            db.ExecuteNonQuery();
            rtVal = true;
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 更新圖徵名稱
    /// </summary>
    public static bool UpdateGeomName(string geomID, string geomName)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
UPDATE [dbo].[OSI_Geom]
SET [GeomName] = @GeomName
WHERE GeomID = @GeomID";

            db.Parameters.Clear();
            db.Parameters.Add("@GeomID", geomID);
            db.Parameters.Add("@GeomName", geomName);

            db.ExecuteNonQuery();
            rtVal = true;
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 更新 OSI_Geom 資料（包含名稱和幾何資料）
    /// </summary>
    public static bool UpdateGeom(OSI_Geom geom)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
UPDATE [dbo].[OSI_Geom]
SET [GeomName] = @GeomName,
    [GeoData] = geometry::STGeomFromText(@GeoData, 3826)
WHERE GeomID = @GeomID AND IsValid = 1";

            db.Parameters.Clear();
            db.Parameters.Add("@GeomID", geom.GeomID);
            db.Parameters.Add("@GeomName", string.IsNullOrEmpty(geom.GeomName) ? "" : geom.GeomName);
            db.Parameters.Add("@GeoData", geom.GeoData);

            db.ExecuteNonQuery();
            rtVal = true;
        }
        catch (Exception ex)
        {
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 使用空間比對尋找相似的圖徵
    /// </summary>
    public static OSI_Geom FindSimilarGeom(int reportID, string wkt, double tolerance = 0.001)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP 1 GeomID, ReportID, GeomName, GeoData.STAsText() AS GeoData, IsValid, CreatedAt, DeletedAt, DeletedBy
FROM OSI_Geom
WHERE ReportID = @ReportID 
    AND IsValid = 1 
    AND DeletedAt IS NULL
    AND GeoData.STDistance(geometry::STGeomFromText(@WKT, 3826)) < @Tolerance
ORDER BY GeoData.STDistance(geometry::STGeomFromText(@WKT, 3826))";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);
        db.Parameters.Add("@WKT", wkt);
        db.Parameters.Add("@Tolerance", tolerance);

        var table = db.GetTable();
        if (table != null && table.Rows.Count > 0)
        {
            var row = table.Rows[0];
            return new OSI_Geom
            {
                GeomID = row["GeomID"]?.ToString() ?? "",
                ReportID = Convert.ToInt32(row["ReportID"]),
                GeomName = row["GeomName"]?.ToString() ?? "",
                GeoData = row["GeoData"]?.ToString() ?? "",
                IsValid = Convert.ToBoolean(row["IsValid"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                DeletedAt = row["DeletedAt"] == DBNull.Value ? null : (DateTime?)row["DeletedAt"],
                DeletedBy = row["DeletedBy"]?.ToString() ?? ""
            };
        }
        return null;
    }

}