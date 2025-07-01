using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 研究調查日期表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_ResearchPeriods", "研究調查日期表", false)]
    public class IOSI_ResearchPeriods : IMeta
    {

        protected int _PeriodID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PeriodID", "PeriodID", DataSource.UN_OPERATE, "", true)]
        public virtual int PeriodID
        {
            get
            {
                return _PeriodID;
            }
            set
            {
                bool isModify = false;
                if (_PeriodID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PeriodID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PeriodID") == -1)
                    {
                        UPDATE_COLUMN.Add("PeriodID");
                    }
                    _PeriodID = value;
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

        protected DateTime _StartDate = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartDate", "StartDate", DataSource.TABLE, "", false)]
        public virtual DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                bool isModify = false;
                if (_StartDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_StartDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartDate") == -1)
                    {
                        UPDATE_COLUMN.Add("StartDate");
                    }
                    _StartDate = value;
                }
            }
        }

        protected DateTime _EndDate = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndDate", "EndDate", DataSource.TABLE, "", false)]
        public virtual DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                bool isModify = false;
                if (_EndDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_EndDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndDate") == -1)
                    {
                        UPDATE_COLUMN.Add("EndDate");
                    }
                    _EndDate = value;
                }
            }
        }

        protected string _PeriodLabel = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PeriodLabel", "PeriodLabel", DataSource.TABLE, "", false)]
        public virtual string PeriodLabel
        {
            get
            {
                return _PeriodLabel;
            }
            set
            {
                bool isModify = false;
                if (_PeriodLabel == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PeriodLabel.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PeriodLabel") == -1)
                    {
                        UPDATE_COLUMN.Add("PeriodLabel");
                    }
                    _PeriodLabel = value;
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
    /// 研究調查日期表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_ResearchPeriods : IOSI_ResearchPeriods
    {
    }

}