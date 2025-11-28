using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class OFSGrantTypeContentIdentifierHelper
{
    public static void insert(GrantTypeContentIdentifier model)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            INSERT INTO [OFS_GrantTypeContentIdentifier] ([TypeID],[CreateTime],[CreateUser])
                               OUTPUT Inserted.ID VALUES (@TypeID, GETDATE(),   @CreateUser)
        ";

        db.Parameters.Add("@TypeID", model.TypeID);
        db.Parameters.Add("@CreateUser", CurrentUser.ID);

        model.ID = int.Parse(db.GetTable().Rows[0]["ID"].ToString());
    }
}
