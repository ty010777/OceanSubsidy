using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;

public class OFSGrantTypeContentLogHelper
{
    public static int count(int typeID = 0)
    {
        DbHelper db = new DbHelper();

        db.CommandText = "SELECT COUNT(*) AS Count FROM [OFS_GrantTypeContentLog]";

        if (typeID > 0)
        {
            db.CommandText += " WHERE [TypeID] = @TypeID";

            db.Parameters.Add("@TypeID", typeID);
        }

        return int.Parse(db.GetTable().Rows[0]["Count"].ToString());
    }

    public static void insert(GrantTypeContentLog model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_GrantTypeContentLog] ([TypeID],[URL],[Method],[Content],[Result],[CreateTime],[CreateUser])
                                           VALUES (@TypeID, @URL, @Method, @Content, @Result, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@URL", model.URL);
        db.Parameters.Add("@Method", model.Method);
        db.Parameters.Add("@Content", model.Content);
        db.Parameters.Add("@Result", model.Result);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        db.ExecuteNonQuery();
    }
}
