using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{

    /// <summary>
    /// OFS角色權限對照表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_RolePermission", "OFS角色權限對照表", false)]
    public class IOFS_RolePermission : IMeta
    {

        protected int _OFS_RolePermissionID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OFS_RolePermissionID", "OFS_RolePermissionID", DataSource.UN_OPERATE, "", true)]
        public virtual int OFS_RolePermissionID
        {
            get
            {
                return _OFS_RolePermissionID;
            }
            set
            {
                bool isModify = false;
                if (_OFS_RolePermissionID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_OFS_RolePermissionID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OFS_RolePermissionID") == -1)
                    {
                        UPDATE_COLUMN.Add("OFS_RolePermissionID");
                    }
                    _OFS_RolePermissionID = value;
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

        protected int _PermissionID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PermissionID", "PermissionID", DataSource.TABLE, "", false)]
        public virtual int PermissionID
        {
            get
            {
                return _PermissionID;
            }
            set
            {
                bool isModify = false;
                if (_PermissionID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PermissionID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PermissionID") == -1)
                    {
                        UPDATE_COLUMN.Add("PermissionID");
                    }
                    _PermissionID = value;
                }
            }
        }

    }

}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// OFS角色權限對照表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_RolePermission : IOFS_RolePermission
    {
    }
}
