using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 研究船風險檢核表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_VesselRiskAssessments", "研究船風險檢核表", false)]
    public class IOSI_VesselRiskAssessments : IMeta
    {

        protected int _AssessmentId = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AssessmentId", "AssessmentId", DataSource.UN_OPERATE, "", true)]
        public virtual int AssessmentId
        {
            get
            {
                return _AssessmentId;
            }
            set
            {
                bool isModify = false;
                if (_AssessmentId == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AssessmentId.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AssessmentId") == -1)
                    {
                        UPDATE_COLUMN.Add("AssessmentId");
                    }
                    _AssessmentId = value;
                }
            }
        }

        protected string _Investigator = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Investigator", "Investigator", DataSource.TABLE, "", false)]
        public virtual string Investigator
        {
            get
            {
                return _Investigator;
            }
            set
            {
                bool isModify = false;
                if (_Investigator == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Investigator.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Investigator") == -1)
                    {
                        UPDATE_COLUMN.Add("Investigator");
                    }
                    _Investigator = value;
                }
            }
        }

        protected string _Unit = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Unit", "Unit", DataSource.TABLE, "", false)]
        public virtual string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                bool isModify = false;
                if (_Unit == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Unit.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Unit") == -1)
                    {
                        UPDATE_COLUMN.Add("Unit");
                    }
                    _Unit = value;
                }
            }
        }

        protected string _Title = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Title", "Title", DataSource.TABLE, "", false)]
        public virtual string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                bool isModify = false;
                if (_Title == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Title.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Title") == -1)
                    {
                        UPDATE_COLUMN.Add("Title");
                    }
                    _Title = value;
                }
            }
        }

        protected DateTime _FormDate = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FormDate", "FormDate", DataSource.TABLE, "", false)]
        public virtual DateTime FormDate
        {
            get
            {
                return _FormDate;
            }
            set
            {
                bool isModify = false;
                if (_FormDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_FormDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FormDate") == -1)
                    {
                        UPDATE_COLUMN.Add("FormDate");
                    }
                    _FormDate = value;
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

        protected string _StartTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartTime", "StartTime", DataSource.TABLE, "", false)]
        public virtual string StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                bool isModify = false;
                if (_StartTime == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_StartTime.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartTime") == -1)
                    {
                        UPDATE_COLUMN.Add("StartTime");
                    }
                    _StartTime = value;
                }
            }
        }

        protected string _StartRemark = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartRemark", "StartRemark", DataSource.TABLE, "", false)]
        public virtual string StartRemark
        {
            get
            {
                return _StartRemark;
            }
            set
            {
                bool isModify = false;
                if (_StartRemark == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_StartRemark.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartRemark") == -1)
                    {
                        UPDATE_COLUMN.Add("StartRemark");
                    }
                    _StartRemark = value;
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

        protected string _EndTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndTime", "EndTime", DataSource.TABLE, "", false)]
        public virtual string EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                bool isModify = false;
                if (_EndTime == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_EndTime.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndTime") == -1)
                    {
                        UPDATE_COLUMN.Add("EndTime");
                    }
                    _EndTime = value;
                }
            }
        }

        protected string _EndRemark = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndRemark", "EndRemark", DataSource.TABLE, "", false)]
        public virtual string EndRemark
        {
            get
            {
                return _EndRemark;
            }
            set
            {
                bool isModify = false;
                if (_EndRemark == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_EndRemark.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndRemark") == -1)
                    {
                        UPDATE_COLUMN.Add("EndRemark");
                    }
                    _EndRemark = value;
                }
            }
        }

        protected int _DurationDays = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DurationDays", "DurationDays", DataSource.TABLE, "", false)]
        public virtual int DurationDays
        {
            get
            {
                return _DurationDays;
            }
            set
            {
                bool isModify = false;
                if (_DurationDays == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_DurationDays.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DurationDays") == -1)
                    {
                        UPDATE_COLUMN.Add("DurationDays");
                    }
                    _DurationDays = value;
                }
            }
        }

        protected string _SurveyAreaName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SurveyAreaName", "SurveyAreaName", DataSource.TABLE, "", false)]
        public virtual string SurveyAreaName
        {
            get
            {
                return _SurveyAreaName;
            }
            set
            {
                bool isModify = false;
                if (_SurveyAreaName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_SurveyAreaName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SurveyAreaName") == -1)
                    {
                        UPDATE_COLUMN.Add("SurveyAreaName");
                    }
                    _SurveyAreaName = value;
                }
            }
        }

        protected string _VoyagePlanAndOperations = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("VoyagePlanAndOperations", "VoyagePlanAndOperations", DataSource.TABLE, "", false)]
        public virtual string VoyagePlanAndOperations
        {
            get
            {
                return _VoyagePlanAndOperations;
            }
            set
            {
                bool isModify = false;
                if (_VoyagePlanAndOperations == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_VoyagePlanAndOperations.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("VoyagePlanAndOperations") == -1)
                    {
                        UPDATE_COLUMN.Add("VoyagePlanAndOperations");
                    }
                    _VoyagePlanAndOperations = value;
                }
            }
        }

        protected int _Q_IsOperationInSensitiveArea = 2;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Q_IsOperationInSensitiveArea", "Q_IsOperationInSensitiveArea", DataSource.TABLE, "", false)]
        public virtual int Q_IsOperationInSensitiveArea
        {
            get
            {
                return _Q_IsOperationInSensitiveArea;
            }
            set
            {
                bool isModify = false;
                if (_Q_IsOperationInSensitiveArea == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Q_IsOperationInSensitiveArea.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Q_IsOperationInSensitiveArea") == -1)
                    {
                        UPDATE_COLUMN.Add("Q_IsOperationInSensitiveArea");
                    }
                    _Q_IsOperationInSensitiveArea = value;
                }
            }
        }

        protected int _Q_IsStayTimeMinimized = 2;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Q_IsStayTimeMinimized", "Q_IsStayTimeMinimized", DataSource.TABLE, "", false)]
        public virtual int Q_IsStayTimeMinimized
        {
            get
            {
                return _Q_IsStayTimeMinimized;
            }
            set
            {
                bool isModify = false;
                if (_Q_IsStayTimeMinimized == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Q_IsStayTimeMinimized.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Q_IsStayTimeMinimized") == -1)
                    {
                        UPDATE_COLUMN.Add("Q_IsStayTimeMinimized");
                    }
                    _Q_IsStayTimeMinimized = value;
                }
            }
        }

        protected int _Q_HasReducedOrRelocatedStation = 2;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Q_HasReducedOrRelocatedStation", "Q_HasReducedOrRelocatedStation", DataSource.TABLE, "", false)]
        public virtual int Q_HasReducedOrRelocatedStation
        {
            get
            {
                return _Q_HasReducedOrRelocatedStation;
            }
            set
            {
                bool isModify = false;
                if (_Q_HasReducedOrRelocatedStation == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Q_HasReducedOrRelocatedStation.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Q_HasReducedOrRelocatedStation") == -1)
                    {
                        UPDATE_COLUMN.Add("Q_HasReducedOrRelocatedStation");
                    }
                    _Q_HasReducedOrRelocatedStation = value;
                }
            }
        }

        protected int _Q_KnowsReportingProcedure = 2;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Q_KnowsReportingProcedure", "Q_KnowsReportingProcedure", DataSource.TABLE, "", false)]
        public virtual int Q_KnowsReportingProcedure
        {
            get
            {
                return _Q_KnowsReportingProcedure;
            }
            set
            {
                bool isModify = false;
                if (_Q_KnowsReportingProcedure == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Q_KnowsReportingProcedure.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Q_KnowsReportingProcedure") == -1)
                    {
                        UPDATE_COLUMN.Add("Q_KnowsReportingProcedure");
                    }
                    _Q_KnowsReportingProcedure = value;
                }
            }
        }

        protected int _Q_HasStrongInterferenceContingencyPlan = 2;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Q_HasStrongInterferenceContingencyPlan", "Q_HasStrongInterferenceContingencyPlan", DataSource.TABLE, "", false)]
        public virtual int Q_HasStrongInterferenceContingencyPlan
        {
            get
            {
                return _Q_HasStrongInterferenceContingencyPlan;
            }
            set
            {
                bool isModify = false;
                if (_Q_HasStrongInterferenceContingencyPlan == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Q_HasStrongInterferenceContingencyPlan.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Q_HasStrongInterferenceContingencyPlan") == -1)
                    {
                        UPDATE_COLUMN.Add("Q_HasStrongInterferenceContingencyPlan");
                    }
                    _Q_HasStrongInterferenceContingencyPlan = value;
                }
            }
        }

        protected DateTime _LastUpdated = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("LastUpdated", "LastUpdated", DataSource.TABLE, "", false)]
        public virtual DateTime LastUpdated
        {
            get
            {
                return _LastUpdated;
            }
            set
            {
                bool isModify = false;
                if (_LastUpdated == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_LastUpdated.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("LastUpdated") == -1)
                    {
                        UPDATE_COLUMN.Add("LastUpdated");
                    }
                    _LastUpdated = value;
                }
            }
        }

        protected int? _LastUpdatedBy = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("LastUpdatedBy", "LastUpdatedBy", DataSource.TABLE, "", false)]
        public virtual int? LastUpdatedBy
        {
            get
            {
                return _LastUpdatedBy;
            }
            set
            {
                bool isModify = false;
                if (_LastUpdatedBy == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_LastUpdatedBy.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("LastUpdatedBy") == -1)
                    {
                        UPDATE_COLUMN.Add("LastUpdatedBy");
                    }
                    _LastUpdatedBy = value;
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

    }


}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 研究船風險檢核表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_VesselRiskAssessments : IOSI_VesselRiskAssessments
    {
    }

}