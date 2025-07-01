using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 單位種類表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_UnitType", "單位種類表", false)]
    public class ISys_UnitType : IMeta
    {

        protected int _TypeID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TypeID", "TypeID", DataSource.UN_OPERATE, "", true)]
        public virtual int TypeID
        {
            get
            {
                return _TypeID;
            }
            set
            {
                bool isModify = false;
                if (_TypeID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_TypeID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TypeID") == -1)
                    {
                        UPDATE_COLUMN.Add("TypeID");
                    }
                    _TypeID = value;
                }
            }
        }

        protected string _TypeName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TypeName", "TypeName", DataSource.TABLE, "", false)]
        public virtual string TypeName
        {
            get
            {
                return _TypeName;
            }
            set
            {
                bool isModify = false;
                if (_TypeName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_TypeName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TypeName") == -1)
                    {
                        UPDATE_COLUMN.Add("TypeName");
                    }
                    _TypeName = value;
                }
            }
        }

    }


}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    /// <summary>
    /// 單位種類表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_UnitType : ISys_UnitType
    {
    }
}


