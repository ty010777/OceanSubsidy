using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIVesselRiskCategoryHelper 的摘要描述
/// </summary>
public class OSIVesselRiskCategoryHelper
{
    public OSIVesselRiskCategoryHelper()
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
SELECT [RiskCategoryId]
    ,[CategoryName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskCategory]
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
SELECT [RiskCategoryId]
    ,[CategoryName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskCategory]
WHERE RiskCategoryId = @RiskCategoryId
";
        db.Parameters.Clear();
        db.Parameters.Add("@RiskCategoryId", id);

        return db.GetTable();
    }

}