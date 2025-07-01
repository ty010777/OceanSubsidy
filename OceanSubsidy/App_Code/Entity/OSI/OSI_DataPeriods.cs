using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 資料時間表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_DataPeriods", "資料時間表", false)]
    public class IOSI_DataPeriods : IMeta
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

        protected string _PeriodYear = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PeriodYear", "PeriodYear", DataSource.TABLE, "", false)]
        public virtual string PeriodYear
        {
            get
            {
                return _PeriodYear;
            }
            set
            {
                bool isModify = false;
                if (_PeriodYear == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PeriodYear.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PeriodYear") == -1)
                    {
                        UPDATE_COLUMN.Add("PeriodYear");
                    }
                    _PeriodYear = value;
                }
            }
        }

        protected string _PeriodQuarter = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PeriodQuarter", "PeriodQuarter", DataSource.TABLE, "", false)]
        public virtual string PeriodQuarter
        {
            get
            {
                return _PeriodQuarter;
            }
            set
            {
                bool isModify = false;
                if (_PeriodQuarter == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PeriodQuarter.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PeriodQuarter") == -1)
                    {
                        UPDATE_COLUMN.Add("PeriodQuarter");
                    }
                    _PeriodQuarter = value;
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

        protected string _Color = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Color", "Color", DataSource.TABLE, "", false)]
        public virtual string Color
        {
            get
            {
                return _Color;
            }
            set
            {
                bool isModify = false;
                if (_Color == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Color.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Color") == -1)
                    {
                        UPDATE_COLUMN.Add("Color");
                    }
                    _Color = value;
                }
            }
        }

        protected DateTime _QuarterStartDate = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("QuarterStartDate", "QuarterStartDate", DataSource.TABLE, "", false)]
        public virtual DateTime QuarterStartDate
        {
            get
            {
                return _QuarterStartDate;
            }
            set
            {
                bool isModify = false;
                if (_QuarterStartDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_QuarterStartDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("QuarterStartDate") == -1)
                    {
                        UPDATE_COLUMN.Add("QuarterStartDate");
                    }
                    _QuarterStartDate = value;
                }
            }
        }

        protected DateTime _QuarterEndDate = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("QuarterEndDate", "QuarterEndDate", DataSource.TABLE, "", false)]
        public virtual DateTime QuarterEndDate
        {
            get
            {
                return _QuarterEndDate;
            }
            set
            {
                bool isModify = false;
                if (_QuarterEndDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_QuarterEndDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("QuarterEndDate") == -1)
                    {
                        UPDATE_COLUMN.Add("QuarterEndDate");
                    }
                    _QuarterEndDate = value;
                }
            }
        }

        protected bool _IsCopy = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsCopy", "IsCopy", DataSource.TABLE, "", false)]
        public virtual bool IsCopy
        {
            get
            {
                return _IsCopy;
            }
            set
            {
                bool isModify = false;
                if (_IsCopy == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_IsCopy.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsCopy") == -1)
                    {
                        UPDATE_COLUMN.Add("IsCopy");
                    }
                    _IsCopy = value;
                }
            }
        }



    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 資料時間表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_DataPeriods : IOSI_DataPeriods
    {
    }

}