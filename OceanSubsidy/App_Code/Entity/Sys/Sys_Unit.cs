using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 系統單位表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_Unit", "系統單位表", false)]
    public class ISys_Unit : IMeta
    {

        protected int _UnitID = 0;
        ///<summary>
        /// PK (PK)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UnitID", "UnitID", DataSource.UN_OPERATE, "PK", true)]
        public virtual int UnitID
        {
            get
            {
                return _UnitID;
            }
            set
            {
                bool isModify = false;
                if (_UnitID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UnitID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UnitID") == -1)
                    {
                        UPDATE_COLUMN.Add("UnitID");
                    }
                    _UnitID = value;
                }
            }
        }

        protected string _UnitName = null;
        ///<summary>
        /// 單位名稱 (單位名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UnitName", "UnitName", DataSource.TABLE, "單位名稱", false)]
        public virtual string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                bool isModify = false;
                if (_UnitName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UnitName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UnitName") == -1)
                    {
                        UPDATE_COLUMN.Add("UnitName");
                    }
                    _UnitName = value;
                }
            }
        }

        protected int? _ParentUnitID = null;
        ///<summary>
        /// 上層單位 (上層單位)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ParentUnitID", "ParentUnitID", DataSource.TABLE, "上層單位", false)]
        public virtual int? ParentUnitID
        {
            get
            {
                return _ParentUnitID;
            }
            set
            {
                bool isModify = false;
                if (_ParentUnitID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ParentUnitID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ParentUnitID") == -1)
                    {
                        UPDATE_COLUMN.Add("ParentUnitID");
                    }
                    _ParentUnitID = value;
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
    /// 系統單位表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_Unit : ISys_Unit
    {
    }

}