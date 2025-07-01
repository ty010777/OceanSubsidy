using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 載具類型表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_CarrierTypes", "載具類型表", false)]
    public class IOSI_CarrierTypes : IMeta
    {

        protected int _CarrierTypeID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CarrierTypeID", "CarrierTypeID", DataSource.UN_OPERATE, "", true)]
        public virtual int CarrierTypeID
        {
            get
            {
                return _CarrierTypeID;
            }
            set
            {
                bool isModify = false;
                if (_CarrierTypeID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CarrierTypeID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CarrierTypeID") == -1)
                    {
                        UPDATE_COLUMN.Add("CarrierTypeID");
                    }
                    _CarrierTypeID = value;
                }
            }
        }

        protected string _CarrierTypeName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CarrierTypeName", "CarrierTypeName", DataSource.TABLE, "", false)]
        public virtual string CarrierTypeName
        {
            get
            {
                return _CarrierTypeName;
            }
            set
            {
                bool isModify = false;
                if (_CarrierTypeName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CarrierTypeName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CarrierTypeName") == -1)
                    {
                        UPDATE_COLUMN.Add("CarrierTypeName");
                    }
                    _CarrierTypeName = value;
                }
            }
        }

    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 載具類型表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_CarrierTypes : IOSI_CarrierTypes
    {
    }

}