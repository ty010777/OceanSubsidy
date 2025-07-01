using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 系統表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_App", "系統表", false)]
    public class ISys_App : IMeta
    {

        protected int _SystemID = 0;
        ///<summary>
        /// PK (PK)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SystemID", "SystemID", DataSource.UN_OPERATE, "PK", true)]
        public virtual int SystemID
        {
            get
            {
                return _SystemID;
            }
            set
            {
                bool isModify = false;
                if (_SystemID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_SystemID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SystemID") == -1)
                    {
                        UPDATE_COLUMN.Add("SystemID");
                    }
                    _SystemID = value;
                }
            }
        }

        protected string _SystemName = null;
        ///<summary>
        /// 系統名稱 (系統名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SystemName", "SystemName", DataSource.TABLE, "系統名稱", false)]
        public virtual string SystemName
        {
            get
            {
                return _SystemName;
            }
            set
            {
                bool isModify = false;
                if (_SystemName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_SystemName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SystemName") == -1)
                    {
                        UPDATE_COLUMN.Add("SystemName");
                    }
                    _SystemName = value;
                }
            }
        }

    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 系統表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_App : ISys_App
    {
    }

}