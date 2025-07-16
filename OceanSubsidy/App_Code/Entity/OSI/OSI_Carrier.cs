using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{

    /// <summary>
    /// 使用載具名稱表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_Carrier", "使用載具名稱表", false)]
    public class IOSI_Carrier : IMeta
    {

        protected int _CarrierID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CarrierID", "CarrierID", DataSource.UN_OPERATE, "", true)]
        public virtual int CarrierID
        {
            get
            {
                return _CarrierID;
            }
            set
            {
                bool isModify = false;
                if (_CarrierID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CarrierID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CarrierID") == -1)
                    {
                        UPDATE_COLUMN.Add("CarrierID");
                    }
                    _CarrierID = value;
                }
            }
        }

        protected int _ReportID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReportID", "ReportID", DataSource.TABLE, "", false)]
        public virtual int ReportID
        {
            get
            {
                return _ReportID;
            }
            set
            {
                bool isModify = false;
                if (_ReportID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ReportID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReportID") == -1)
                    {
                        UPDATE_COLUMN.Add("ReportID");
                    }
                    _ReportID = value;
                }
            }
        }

        protected int? _CarrierTypeID = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CarrierTypeID", "CarrierTypeID", DataSource.TABLE, "", false)]
        public virtual int? CarrierTypeID
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

        protected string _CarrierDetail = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CarrierDetail", "CarrierDetail", DataSource.TABLE, "", false)]
        public virtual string CarrierDetail
        {
            get
            {
                return _CarrierDetail;
            }
            set
            {
                bool isModify = false;
                if (_CarrierDetail == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CarrierDetail.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CarrierDetail") == -1)
                    {
                        UPDATE_COLUMN.Add("CarrierDetail");
                    }
                    _CarrierDetail = value;
                }
            }
        }

        protected string _CarrierNo = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CarrierNo", "CarrierNo", DataSource.TABLE, "", false)]
        public virtual string CarrierNo
        {
            get
            {
                return _CarrierNo;
            }
            set
            {
                bool isModify = false;
                if (_CarrierNo == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CarrierNo.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CarrierNo") == -1)
                    {
                        UPDATE_COLUMN.Add("CarrierNo");
                    }
                    _CarrierNo = value;
                }
            }
        }

        protected bool _IsValid = true;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "", false)]
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

        protected DateTime _CreatedAt = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreatedAt", "CreatedAt", DataSource.TABLE, "", false)]
        public virtual DateTime CreatedAt
        {
            get
            {
                return _CreatedAt;
            }
            set
            {
                bool isModify = false;
                if (_CreatedAt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CreatedAt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreatedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("CreatedAt");
                    }
                    _CreatedAt = value;
                }
            }
        }

        protected DateTime? _DeletedAt = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DeletedAt", "DeletedAt", DataSource.TABLE, "", false)]
        public virtual DateTime? DeletedAt
        {
            get
            {
                return _DeletedAt;
            }
            set
            {
                bool isModify = false;
                if (_DeletedAt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_DeletedAt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DeletedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("DeletedAt");
                    }
                    _DeletedAt = value;
                }
            }
        }

        protected string _DeletedBy = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DeletedBy", "DeletedBy", DataSource.TABLE, "", false)]
        public virtual string DeletedBy
        {
            get
            {
                return _DeletedBy;
            }
            set
            {
                bool isModify = false;
                if (_DeletedBy == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_DeletedBy.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DeletedBy") == -1)
                    {
                        UPDATE_COLUMN.Add("DeletedBy");
                    }
                    _DeletedBy = value;
                }
            }
        }

    }




}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 使用載具名稱表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_Carrier : IOSI_Carrier
    {
    }

}


