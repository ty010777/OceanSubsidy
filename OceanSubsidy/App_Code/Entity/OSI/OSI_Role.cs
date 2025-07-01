using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 海洋科學調查_角色表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_Role", "海洋科學調查_角色表", false)]
    public class IOSI_Role : IMeta
    {

        protected int _RoleID = 0;
        ///<summary>
        /// PK (PK)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RoleID", "RoleID", DataSource.UN_OPERATE, "PK", true)]
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

        protected string _RoleName = null;
        ///<summary>
        /// 系統名稱 (系統名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RoleName", "RoleName", DataSource.TABLE, "系統名稱", false)]
        public virtual string RoleName
        {
            get
            {
                return _RoleName;
            }
            set
            {
                bool isModify = false;
                if (_RoleName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_RoleName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("RoleName") == -1)
                    {
                        UPDATE_COLUMN.Add("RoleName");
                    }
                    _RoleName = value;
                }
            }
        }

        protected int _Sort = 0;
        ///<summary>
        /// 排序 (排序)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Sort", "Sort", DataSource.TABLE, "排序", false)]
        public virtual int Sort
        {
            get
            {
                return _Sort;
            }
            set
            {
                bool isModify = false;
                if (_Sort == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Sort.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Sort") == -1)
                    {
                        UPDATE_COLUMN.Add("Sort");
                    }
                    _Sort = value;
                }
            }
        }

        protected bool _IsValid = false;
        ///<summary>
        /// 假刪除 (假刪除)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "假刪除", false)]
        public virtual bool IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                bool isModify = false;
                if (_IsValid == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_IsValid.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsValid") == -1)
                    {
                        UPDATE_COLUMN.Add("IsValid");
                    }
                    _IsValid = value;
                }
            }
        }

    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 海洋科學調查_角色表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_Role : IOSI_Role
    {
    }

}