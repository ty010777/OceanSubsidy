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
        /// 計畫ID ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProjectID", "ProjectID", DataSource.TABLE, "計畫ID", false)]
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
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("WorkItem_id", "WorkItem_id", DataSource.TABLE, "", false)]
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
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("WorkName", "WorkName", DataSource.TABLE, "", false)]
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
        
        protected int? _StartMonth = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartMonth", "StartMonth", DataSource.TABLE, "", false)]
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
        
        protected int? _EndMonth = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndMonth", "EndMonth", DataSource.TABLE, "", false)]
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
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Weighting", "Weighting", DataSource.TABLE, "", false)]
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
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("InvestMonth", "InvestMonth", DataSource.TABLE, "", false)]
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
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsOutsourced", "IsOutsourced", DataSource.TABLE, "", false)]
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