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
    /// 科專申請表-期程/工作項目/查核 (期程及工作項目-子表)
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_WorkSch_Main", "科專申請表-期程/工作項目/查核", false)]
    public class IOFS_SCI_WorkSch_Main : IMeta
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
        
        protected string _WorkItem_id = "";
        ///<summary>
        /// 編號 (編號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("WorkItem_id", "WorkItem_id", DataSource.TABLE, "編號", false)]
        public virtual string WorkItem_id
        {
            get
            {
                return _WorkItem_id;
            }
            set
            {
                bool isModify = false;
                if (_WorkItem_id == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_WorkItem_id.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("WorkItem_id") == -1)
                    {
                        UPDATE_COLUMN.Add("WorkItem_id");
                    }
                    _WorkItem_id = value;
                }
            }
        }
        
        protected string _WorkName = "";
        ///<summary>
        /// 工作項目 (工作項目)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("WorkName", "WorkName", DataSource.TABLE, "工作項目", false)]
        public virtual string WorkName
        {
            get
            {
                return _WorkName;
            }
            set
            {
                bool isModify = false;
                if (_WorkName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_WorkName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("WorkName") == -1)
                    {
                        UPDATE_COLUMN.Add("WorkName");
                    }
                    _WorkName = value;
                }
            }
        }
        
        protected int? _StartYear = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartYear", "StartYear", DataSource.TABLE, "", false)]
        public virtual int? StartYear
        {
            get
            {
                return _StartYear;
            }
            set
            {
                bool isModify = false;
                if (_StartYear == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StartYear.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartYear") == -1)
                    {
                        UPDATE_COLUMN.Add("StartYear");
                    }
                    _StartYear = value;
                }
            }
        }
        
        protected int? _StartMonth = null;
        ///<summary>
        /// 起訖月份 (起訖月份)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartMonth", "StartMonth", DataSource.TABLE, "起訖月份", false)]
        public virtual int? StartMonth
        {
            get
            {
                return _StartMonth;
            }
            set
            {
                bool isModify = false;
                if (_StartMonth == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StartMonth.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartMonth") == -1)
                    {
                        UPDATE_COLUMN.Add("StartMonth");
                    }
                    _StartMonth = value;
                }
            }
        }
        
        protected int? _EndYear = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndYear", "EndYear", DataSource.TABLE, "", false)]
        public virtual int? EndYear
        {
            get
            {
                return _EndYear;
            }
            set
            {
                bool isModify = false;
                if (_EndYear == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EndYear.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndYear") == -1)
                    {
                        UPDATE_COLUMN.Add("EndYear");
                    }
                    _EndYear = value;
                }
            }
        }
        
        protected int? _EndMonth = null;
        ///<summary>
        /// 起訖月份 (起訖月份)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndMonth", "EndMonth", DataSource.TABLE, "起訖月份", false)]
        public virtual int? EndMonth
        {
            get
            {
                return _EndMonth;
            }
            set
            {
                bool isModify = false;
                if (_EndMonth == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EndMonth.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndMonth") == -1)
                    {
                        UPDATE_COLUMN.Add("EndMonth");
                    }
                    _EndMonth = value;
                }
            }
        }
        
        protected decimal? _Weighting = null;
        ///<summary>
        /// 權重 (權重)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Weighting", "Weighting", DataSource.TABLE, "權重", false)]
        public virtual decimal? Weighting
        {
            get
            {
                return _Weighting;
            }
            set
            {
                bool isModify = false;
                if (_Weighting == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Weighting.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Weighting") == -1)
                    {
                        UPDATE_COLUMN.Add("Weighting");
                    }
                    _Weighting = value;
                }
            }
        }
        
        protected decimal? _InvestMonth = null;
        ///<summary>
        /// 投入人月數 (投入人月數)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("InvestMonth", "InvestMonth", DataSource.TABLE, "投入人月數", false)]
        public virtual decimal? InvestMonth
        {
            get
            {
                return _InvestMonth;
            }
            set
            {
                bool isModify = false;
                if (_InvestMonth == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_InvestMonth.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("InvestMonth") == -1)
                    {
                        UPDATE_COLUMN.Add("InvestMonth");
                    }
                    _InvestMonth = value;
                }
            }
        }
        
        protected bool? _IsOutsourced = false;
        ///<summary>
        /// 是否委外 (是否委外)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsOutsourced", "IsOutsourced", DataSource.TABLE, "是否委外", false)]
        public virtual bool? IsOutsourced
        {
            get
            {
                return _IsOutsourced;
            }
            set
            {
                bool isModify = false;
                if (_IsOutsourced == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsOutsourced.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsOutsourced") == -1)
                    {
                        UPDATE_COLUMN.Add("IsOutsourced");
                    }
                    _IsOutsourced = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專申請表-期程/工作項目/查核 (期程及工作項目-子表)
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_WorkSch_Main : IOFS_SCI_WorkSch_Main
    {
    }
    
}