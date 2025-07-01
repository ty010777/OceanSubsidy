using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    ///  ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_RolePermission", "", false)]
    public class IOSI_RolePermission : IMeta
    {

        protected int _OSI_RolePermissionID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OSI_RolePermissionID", "OSI_RolePermissionID", DataSource.UN_OPERATE, "", true)]
        public virtual int OSI_RolePermissionID
        {
            get
            {
                return _OSI_RolePermissionID;
            }
            set
            {
                bool isModify = false;
                if (_OSI_RolePermissionID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_OSI_RolePermissionID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OSI_RolePermissionID") == -1)
                    {
                        UPDATE_COLUMN.Add("OSI_RolePermissionID");
                    }
                    _OSI_RolePermissionID = value;
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
    ///  ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_RolePermission : IOSI_RolePermission
    {
    }

}