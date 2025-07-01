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
    [GisTableAttribute("Sys_Permission", "", false)]
    public class ISys_Permission : IMeta
    {

        protected int _PermissionID = 0;
        ///<summary>
        /// PK (PK)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PermissionID", "PermissionID", DataSource.UN_OPERATE, "PK", true)]
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

        protected string _PermissionCode = null;
        ///<summary>
        /// 判斷用的代碼 (判斷用的代碼)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PermissionCode", "PermissionCode", DataSource.TABLE, "判斷用的代碼", false)]
        public virtual string PermissionCode
        {
            get
            {
                return _PermissionCode;
            }
            set
            {
                bool isModify = false;
                if (_PermissionCode == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PermissionCode.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PermissionCode") == -1)
                    {
                        UPDATE_COLUMN.Add("PermissionCode");
                    }
                    _PermissionCode = value;
                }
            }
        }

        protected string _PermissionName = null;
        ///<summary>
        /// 名稱 (名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PermissionName", "PermissionName", DataSource.TABLE, "名稱", false)]
        public virtual string PermissionName
        {
            get
            {
                return _PermissionName;
            }
            set
            {
                bool isModify = false;
                if (_PermissionName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PermissionName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PermissionName") == -1)
                    {
                        UPDATE_COLUMN.Add("PermissionName");
                    }
                    _PermissionName = value;
                }
            }
        }

        protected string _Url = "";
        ///<summary>
        /// 頁面路徑 (頁面路徑)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Url", "Url", DataSource.TABLE, "頁面路徑", false)]
        public virtual string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                bool isModify = false;
                if (_Url == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Url.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Url") == -1)
                    {
                        UPDATE_COLUMN.Add("Url");
                    }
                    _Url = value;
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
    public partial class Sys_Permission : ISys_Permission
    {
    }

}