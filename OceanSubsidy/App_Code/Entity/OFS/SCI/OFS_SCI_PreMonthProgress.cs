using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Runtime.Serialization;
using GS.Data;
using GS.Extension;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    
    /// <summary>
    ///  ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_PreMonthProgress", "", false)]
    public class IOFS_SCI_PreMonthProgress : IMeta
    {
        
        protected string _ProjectID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProjectID", "ProjectID", DataSource.TABLE, "", false)]
        public virtual string ProjectID
        {
            get
            {
                return _ProjectID;
            }
            set
            {
                bool isModify = false;
                if (_ProjectID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ProjectID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ProjectID") == -1)
                    {
                        UPDATE_COLUMN.Add("ProjectID");
                    }
                    _ProjectID = value;
                }
            }
        }
        
        protected string _Month = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Month", "Month", DataSource.TABLE, "", false)]
        public virtual string Month
        {
            get
            {
                return _Month;
            }
            set
            {
                bool isModify = false;
                if (_Month == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Month.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Month") == -1)
                    {
                        UPDATE_COLUMN.Add("Month");
                    }
                    _Month = value;
                }
            }
        }
        
        protected string _PreWorkAbstract = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PreWorkAbstract", "PreWorkAbstract", DataSource.TABLE, "", false)]
        public virtual string PreWorkAbstract
        {
            get
            {
                return _PreWorkAbstract;
            }
            set
            {
                bool isModify = false;
                if (_PreWorkAbstract == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PreWorkAbstract.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PreWorkAbstract") == -1)
                    {
                        UPDATE_COLUMN.Add("PreWorkAbstract");
                    }
                    _PreWorkAbstract = value;
                }
            }
        }
        
        protected string _ActWorkAbstract = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ActWorkAbstract", "ActWorkAbstract", DataSource.TABLE, "", false)]
        public virtual string ActWorkAbstract
        {
            get
            {
                return _ActWorkAbstract;
            }
            set
            {
                bool isModify = false;
                if (_ActWorkAbstract == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ActWorkAbstract.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ActWorkAbstract") == -1)
                    {
                        UPDATE_COLUMN.Add("ActWorkAbstract");
                    }
                    _ActWorkAbstract = value;
                }
            }
        }
        
        protected string _CheckDescription = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CheckDescription", "CheckDescription", DataSource.TABLE, "", false)]
        public virtual string CheckDescription
        {
            get
            {
                return _CheckDescription;
            }
            set
            {
                bool isModify = false;
                if (_CheckDescription == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CheckDescription.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CheckDescription") == -1)
                    {
                        UPDATE_COLUMN.Add("CheckDescription");
                    }
                    _CheckDescription = value;
                }
            }
        }
        
        protected decimal? _PreProgress = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PreProgress", "PreProgress", DataSource.TABLE, "", false)]
        public virtual decimal? PreProgress
        {
            get
            {
                return _PreProgress;
            }
            set
            {
                bool isModify = false;
                if (_PreProgress == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PreProgress.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PreProgress") == -1)
                    {
                        UPDATE_COLUMN.Add("PreProgress");
                    }
                    _PreProgress = value;
                }
            }
        }
        
        protected decimal? _ActProgress = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ActProgress", "ActProgress", DataSource.TABLE, "", false)]
        public virtual decimal? ActProgress
        {
            get
            {
                return _ActProgress;
            }
            set
            {
                bool isModify = false;
                if (_ActProgress == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ActProgress.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ActProgress") == -1)
                    {
                        UPDATE_COLUMN.Add("ActProgress");
                    }
                    _ActProgress = value;
                }
            }
        }
        
        protected decimal? _MonthlySubsidy = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("MonthlySubsidy", "MonthlySubsidy", DataSource.TABLE, "", false)]
        public virtual decimal? MonthlySubsidy
        {
            get
            {
                return _MonthlySubsidy;
            }
            set
            {
                bool isModify = false;
                if (_MonthlySubsidy == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_MonthlySubsidy.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("MonthlySubsidy") == -1)
                    {
                        UPDATE_COLUMN.Add("MonthlySubsidy");
                    }
                    _MonthlySubsidy = value;
                }
            }
        }
        
        protected decimal? _MonthlyCoop = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("MonthlyCoop", "MonthlyCoop", DataSource.TABLE, "", false)]
        public virtual decimal? MonthlyCoop
        {
            get
            {
                return _MonthlyCoop;
            }
            set
            {
                bool isModify = false;
                if (_MonthlyCoop == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_MonthlyCoop.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("MonthlyCoop") == -1)
                    {
                        UPDATE_COLUMN.Add("MonthlyCoop");
                    }
                    _MonthlyCoop = value;
                }
            }
        }
        
        protected decimal? _MonthlyTotal = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("MonthlyTotal", "MonthlyTotal", DataSource.UN_OPERATE, "", false)]
        public virtual decimal? MonthlyTotal
        {
            get
            {
                return _MonthlyTotal;
            }
            set
            {
                bool isModify = false;
                if (_MonthlyTotal == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_MonthlyTotal.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("MonthlyTotal") == -1)
                    {
                        UPDATE_COLUMN.Add("MonthlyTotal");
                    }
                    _MonthlyTotal = value;
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
    public partial class OFS_SCI_PreMonthProgress : IOFS_SCI_PreMonthProgress
    {
    }
    
}