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
    [GisTableAttribute("OFS_CLB_Application_Plan", "", false)]
    public class IOFS_CLB_Application_Plan : IMeta
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
        
        protected DateTime? _StartDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartDate", "StartDate", DataSource.TABLE, "", false)]
        public virtual DateTime? StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                bool isModify = false;
                if (_StartDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StartDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartDate") == -1)
                    {
                        UPDATE_COLUMN.Add("StartDate");
                    }
                    _StartDate = value;
                }
            }
        }
        
        protected DateTime? _EndDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndDate", "EndDate", DataSource.TABLE, "", false)]
        public virtual DateTime? EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                bool isModify = false;
                if (_EndDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EndDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndDate") == -1)
                    {
                        UPDATE_COLUMN.Add("EndDate");
                    }
                    _EndDate = value;
                }
            }
        }
        
        protected string _Purpose = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Purpose", "Purpose", DataSource.TABLE, "", false)]
        public virtual string Purpose
        {
            get
            {
                return _Purpose;
            }
            set
            {
                bool isModify = false;
                if (_Purpose == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Purpose.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Purpose") == -1)
                    {
                        UPDATE_COLUMN.Add("Purpose");
                    }
                    _Purpose = value;
                }
            }
        }
        
        protected string _PlanContent = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PlanContent", "PlanContent", DataSource.TABLE, "", false)]
        public virtual string PlanContent
        {
            get
            {
                return _PlanContent;
            }
            set
            {
                bool isModify = false;
                if (_PlanContent == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PlanContent.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PlanContent") == -1)
                    {
                        UPDATE_COLUMN.Add("PlanContent");
                    }
                    _PlanContent = value;
                }
            }
        }
        
        protected string _PreBenefits = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PreBenefits", "PreBenefits", DataSource.TABLE, "", false)]
        public virtual string PreBenefits
        {
            get
            {
                return _PreBenefits;
            }
            set
            {
                bool isModify = false;
                if (_PreBenefits == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PreBenefits.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PreBenefits") == -1)
                    {
                        UPDATE_COLUMN.Add("PreBenefits");
                    }
                    _PreBenefits = value;
                }
            }
        }
        
        protected string _PlanLocation = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PlanLocation", "PlanLocation", DataSource.TABLE, "", false)]
        public virtual string PlanLocation
        {
            get
            {
                return _PlanLocation;
            }
            set
            {
                bool isModify = false;
                if (_PlanLocation == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PlanLocation.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PlanLocation") == -1)
                    {
                        UPDATE_COLUMN.Add("PlanLocation");
                    }
                    _PlanLocation = value;
                }
            }
        }
        
        protected string _EstimatedPeople = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EstimatedPeople", "EstimatedPeople", DataSource.TABLE, "", false)]
        public virtual string EstimatedPeople
        {
            get
            {
                return _EstimatedPeople;
            }
            set
            {
                bool isModify = false;
                if (_EstimatedPeople == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EstimatedPeople.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EstimatedPeople") == -1)
                    {
                        UPDATE_COLUMN.Add("EstimatedPeople");
                    }
                    _EstimatedPeople = value;
                }
            }
        }
        
        protected string _EmergencyPlan = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EmergencyPlan", "EmergencyPlan", DataSource.TABLE, "", false)]
        public virtual string EmergencyPlan
        {
            get
            {
                return _EmergencyPlan;
            }
            set
            {
                bool isModify = false;
                if (_EmergencyPlan == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EmergencyPlan.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EmergencyPlan") == -1)
                    {
                        UPDATE_COLUMN.Add("EmergencyPlan");
                    }
                    _EmergencyPlan = value;
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
    public partial class OFS_CLB_Application_Plan : IOFS_CLB_Application_Plan
    {
    }
    
}