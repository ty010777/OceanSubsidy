using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIVesselFilesHelper 的摘要描述
/// </summary>
public class OSIVesselFilesHelper
{
    public OSIVesselFilesHelper()
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
SELECT [AttachmentID]
    ,[AssessmentId]
    ,[FileName]
    ,[FilePath]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
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
SELECT [AttachmentID]
    ,[AssessmentId]
    ,[FileName]
    ,[FilePath]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
WHERE IsValid = 1
AND AttachmentID = @AttachmentID
";
        db.Parameters.Clear();
        db.Parameters.Add("@AttachmentID", id);

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
SELECT [AttachmentID]
    ,[AssessmentId]
    ,[FileName]
    ,[FilePath]
    ,[IsValid]
    ,[CreatedAt]
    ,[DeletedAt]
    ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
WHERE IsValid = 1 AND DeletedAt IS NULL
AND AssessmentId = @AssessmentId
ORDER BY CreatedAt
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);

        return db.GetTable();
    }

    /// <summary>
    /// 軟刪除特定 AssessmentId 的所有附件
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <param name="deletedBy"></param>
    /// <returns></returns>
    public static bool SoftDeleteByAssessmentId(int assessmentId, string deletedBy)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
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
    /// 軟刪除特定附件
    /// </summary>
    /// <param name="attachmentId"></param>
    /// <param name="deletedBy"></param>
    /// <returns></returns>
    public static bool SoftDelete(int attachmentId, string deletedBy)
    {
        bool rtVal = true;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
SET IsValid = 0, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
WHERE AttachmentID = @AttachmentID AND IsValid = 1
";
        db.Parameters.Clear();
        db.Parameters.Add("@AttachmentID", attachmentId);
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
    /// 新增附件
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static int Insert(OSI_VesselFiles file)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
    ([AssessmentId], [FileName], [FilePath], [IsValid], [CreatedAt])
VALUES
    (@AssessmentId, @FileName, @FilePath, @IsValid, @CreatedAt);
SELECT SCOPE_IDENTITY();
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", file.AssessmentId);
        db.Parameters.Add("@FileName", file.FileName);
        db.Parameters.Add("@FilePath", file.FilePath);
        db.Parameters.Add("@IsValid", file.IsValid);
        db.Parameters.Add("@CreatedAt", file.CreatedAt);

        var result = db.GetTable();
        if (result != null && result.Rows.Count > 0)
        {
            return Convert.ToInt32(result.Rows[0][0]);
        }
        return 0;
    }

    /// <summary>
    /// 批次新增附件
    /// </summary>
    /// <param name="assessmentId"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    public static bool InsertBatch(int assessmentId, List<OSI_VesselFiles> files)
    {
        if (files == null || files.Count == 0)
            return true;

        DbHelper db = new DbHelper();
        db.BeginTrans();
        try
        {
            foreach (var file in files)
            {
                db.CommandText =
                    @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
    ([AssessmentId], [FileName], [FilePath], [IsValid], [CreatedAt])
VALUES
    (@AssessmentId, @FileName, @FilePath, 1, GETDATE())
";
                db.Parameters.Clear();
                db.Parameters.Add("@AssessmentId", assessmentId);
                db.Parameters.Add("@FileName", file.FileName);
                db.Parameters.Add("@FilePath", file.FilePath);
                db.ExecuteNonQuery();
            }

            db.Commit();
            return true;
        }
        catch
        {
            db.Rollback();
            return false;
        }
    }

}