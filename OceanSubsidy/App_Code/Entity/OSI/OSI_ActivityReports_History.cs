using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 活動填報表歷史紀錄 (只有在非時間內修改才有紀錄)
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_ActivityReports_History", "活動填報表歷史紀錄", false)]
    public class IOSI_ActivityReports_History : IMeta
    {

        protected int _HistoryID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("HistoryID", "HistoryID", DataSource.UN_OPERATE, "", true)]
        public virtual int HistoryID
        {
            get
            {
                return _HistoryID;
            }
            set
            {
                bool isModify = false;
                if (_HistoryID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_HistoryID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("HistoryID") == -1)
                    {
                        UPDATE_COLUMN.Add("HistoryID");
                    }
                    _HistoryID = value;
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

        protected int _PeriodID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PeriodID", "PeriodID", DataSource.TABLE, "", false)]
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

        protected int? _ReportingUnitID = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReportingUnitID", "ReportingUnitID", DataSource.TABLE, "", false)]
        public virtual int? ReportingUnitID
        {
            get
            {
                return _ReportingUnitID;
            }
            set
            {
                bool isModify = false;
                if (_ReportingUnitID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ReportingUnitID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReportingUnitID") == -1)
                    {
                        UPDATE_COLUMN.Add("ReportingUnitID");
                    }
                    _ReportingUnitID = value;
                }
            }
        }

        protected string _ActivityName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ActivityName", "ActivityName", DataSource.TABLE, "", false)]
        public virtual string ActivityName
        {
            get
            {
                return _ActivityName;
            }
            set
            {
                bool isModify = false;
                if (_ActivityName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ActivityName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ActivityName") == -1)
                    {
                        UPDATE_COLUMN.Add("ActivityName");
                    }
                    _ActivityName = value;
                }
            }
        }

        protected int _NatureID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("NatureID", "NatureID", DataSource.TABLE, "", false)]
        public virtual int NatureID
        {
            get
            {
                return _NatureID;
            }
            set
            {
                bool isModify = false;
                if (_NatureID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_NatureID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("NatureID") == -1)
                    {
                        UPDATE_COLUMN.Add("NatureID");
                    }
                    _NatureID = value;
                }
            }
        }

        protected string _NatureText = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("NatureText", "NatureText", DataSource.TABLE, "", false)]
        public virtual string NatureText
        {
            get
            {
                return _NatureText;
            }
            set
            {
                bool isModify = false;
                if (_NatureText == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_NatureText.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("NatureText") == -1)
                    {
                        UPDATE_COLUMN.Add("NatureText");
                    }
                    _NatureText = value;
                }
            }
        }

        protected int? _ResearchItemID = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ResearchItemID", "ResearchItemID", DataSource.TABLE, "", false)]
        public virtual int? ResearchItemID
        {
            get
            {
                return _ResearchItemID;
            }
            set
            {
                bool isModify = false;
                if (_ResearchItemID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ResearchItemID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ResearchItemID") == -1)
                    {
                        UPDATE_COLUMN.Add("ResearchItemID");
                    }
                    _ResearchItemID = value;
                }
            }
        }

        protected string _ResearchItemNote = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ResearchItemNote", "ResearchItemNote", DataSource.TABLE, "", false)]
        public virtual string ResearchItemNote
        {
            get
            {
                return _ResearchItemNote;
            }
            set
            {
                bool isModify = false;
                if (_ResearchItemNote == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ResearchItemNote.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ResearchItemNote") == -1)
                    {
                        UPDATE_COLUMN.Add("ResearchItemNote");
                    }
                    _ResearchItemNote = value;
                }
            }
        }

        protected string _Instruments = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Instruments", "Instruments", DataSource.TABLE, "", false)]
        public virtual string Instruments
        {
            get
            {
                return _Instruments;
            }
            set
            {
                bool isModify = false;
                if (_Instruments == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Instruments.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Instruments") == -1)
                    {
                        UPDATE_COLUMN.Add("Instruments");
                    }
                    _Instruments = value;
                }
            }
        }

        protected string _ActivityOverview = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ActivityOverview", "ActivityOverview", DataSource.TABLE, "", false)]
        public virtual string ActivityOverview
        {
            get
            {
                return _ActivityOverview;
            }
            set
            {
                bool isModify = false;
                if (_ActivityOverview == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ActivityOverview.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ActivityOverview") == -1)
                    {
                        UPDATE_COLUMN.Add("ActivityOverview");
                    }
                    _ActivityOverview = value;
                }
            }
        }

        protected string _GeoData = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("GeoData", "GeoData", DataSource.TABLE, "", false)]
        public virtual string GeoData
        {
            get
            {
                return _GeoData;
            }
            set
            {
                bool isModify = false;
                if (_GeoData == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_GeoData.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("GeoData") == -1)
                    {
                        UPDATE_COLUMN.Add("GeoData");
                    }
                    _GeoData = value;
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

        protected bool _IsValid = false;
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

        protected DateTime _AuditAt = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AuditAt", "AuditAt", DataSource.TABLE, "", false)]
        public virtual DateTime AuditAt
        {
            get
            {
                return _AuditAt;
            }
            set
            {
                bool isModify = false;
                if (_AuditAt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AuditAt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AuditAt") == -1)
                    {
                        UPDATE_COLUMN.Add("AuditAt");
                    }
                    _AuditAt = value;
                }
            }
        }

        protected string _AuditBy = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AuditBy", "AuditBy", DataSource.TABLE, "", false)]
        public virtual string AuditBy
        {
            get
            {
                return _AuditBy;
            }
            set
            {
                bool isModify = false;
                if (_AuditBy == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AuditBy.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AuditBy") == -1)
                    {
                        UPDATE_COLUMN.Add("AuditBy");
                    }
                    _AuditBy = value;
                }
            }
        }

        protected int? _CopyReportID = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CopyReportID", "CopyReportID", DataSource.TABLE, "", false)]
        public virtual int? CopyReportID
        {
            get
            {
                return _CopyReportID;
            }
            set
            {
                bool isModify = false;
                if (_CopyReportID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CopyReportID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CopyReportID") == -1)
                    {
                        UPDATE_COLUMN.Add("CopyReportID");
                    }
                    _CopyReportID = value;
                }
            }
        }

        protected string _CorrectionNotes = "";
        ///<summary>
        /// 修正說明 ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CorrectionNotes", "CorrectionNotes", DataSource.TABLE, "", false)]
        public virtual string CorrectionNotes
        {
            get
            {
                return _CorrectionNotes;
            }
            set
            {
                bool isModify = false;
                if (_CorrectionNotes == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CorrectionNotes.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CorrectionNotes") == -1)
                    {
                        UPDATE_COLUMN.Add("CorrectionNotes");
                    }
                    _CorrectionNotes = value;
                }
            }
        }



    }

}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 活動填報表歷史紀錄 (只有在非時間內修改才有紀錄)
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_ActivityReports_History : IOSI_ActivityReports_History
    {
    }

}
