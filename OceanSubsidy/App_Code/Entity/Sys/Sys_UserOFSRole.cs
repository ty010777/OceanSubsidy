using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 帳號與OFS角色對照表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_UserOFSRole", "帳號與OFS角色對照表", false)]
    public class ISys_UserOFSRole : IMeta
    {

        protected int _UserOFSRoleID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserOFSRoleID", "UserOFSRoleID", DataSource.UN_OPERATE, "", true)]
        public virtual int UserOFSRoleID
        {
            get
            {
                return _UserOFSRoleID;
            }
            set
            {
                bool isModify = false;
                if (_UserOFSRoleID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UserOFSRoleID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UserOFSRoleID") == -1)
                    {
                        UPDATE_COLUMN.Add("UserOFSRoleID");
                    }
                    _UserOFSRoleID = value;
                }
            }
        }

        protected int _UserID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserID", "UserID", DataSource.TABLE, "", false)]
        public virtual int UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                bool isModify = false;
                if (_UserID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UserID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UserID") == -1)
                    {
                        UPDATE_COLUMN.Add("UserID");
                    }
                    _UserID = value;
                }
            }
        }

        protected int _RoleID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RoleID", "RoleID", DataSource.TABLE, "", false)]
        public virtual int RoleID
        {
            get
            {
                return _RoleID;
            }
            set
            {
                bool isModify = false;
                if (_RoleID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_RoleID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("RoleID") == -1)
                    {
                        UPDATE_COLUMN.Add("RoleID");
                    }
                    _RoleID = value;
                }
            }
        }

    }

}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 帳號與OFS角色對照表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_UserOFSRole : ISys_UserOFSRole
    {
    }
}
