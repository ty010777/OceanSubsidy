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
    [GisTableAttribute("OFS_SCI_WorkSch_CheckStandard", "", false)]
    public class IOFS_SCI_WorkSch_CheckStandard : IMeta
    {
        
        protected int _Id = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Id", "Id", DataSource.UN_OPERATE, "", true)]
        public virtual int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                bool isModify = false;
                if (_Id == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Id.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Id") == -1)
                    {
                        UPDATE_COLUMN.Add("Id");
                    }
                    _Id = value;
                }
            }
        }
        
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
        
        protected string _WorkItem = "";
        ///<summary>
        /// 對應工項 (對應工項)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("WorkItem", "WorkItem", DataSource.TABLE, "對應工項", false)]
        public virtual string WorkItem
        {
            get
            {
                return _WorkItem;
            }
            set
            {
                bool isModify = false;
                if (_WorkItem == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_WorkItem.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("WorkItem") == -1)
                    {
                        UPDATE_COLUMN.Add("WorkItem");
                    }
                    _WorkItem = value;
                }
            }
        }
        
        protected string _SerialNumber = "";
        ///<summary>
        /// 編號 (編號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SerialNumber", "SerialNumber", DataSource.TABLE, "編號", false)]
        public virtual string SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
            set
            {
                bool isModify = false;
                if (_SerialNumber == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SerialNumber.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SerialNumber") == -1)
                    {
                        UPDATE_COLUMN.Add("SerialNumber");
                    }
                    _SerialNumber = value;
                }
            }
        }
        
        protected DateTime? _PlannedFinishDate = null;
        ///<summary>
        /// 預定完成日期 (預定完成日期)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PlannedFinishDate", "PlannedFinishDate", DataSource.TABLE, "預定完成日期", false)]
        public virtual DateTime? PlannedFinishDate
        {
            get
            {
                return _PlannedFinishDate;
            }
            set
            {
                bool isModify = false;
                if (_PlannedFinishDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PlannedFinishDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PlannedFinishDate") == -1)
                    {
                        UPDATE_COLUMN.Add("PlannedFinishDate");
                    }
                    _PlannedFinishDate = value;
                }
            }
        }
        
        protected string _CheckDescription = "";
        ///<summary>
        /// 查核內容概述 (查核內容概述)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CheckDescription", "CheckDescription", DataSource.TABLE, "查核內容概述", false)]
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
        
        protected DateTime? _ActFinishTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ActFinishTime", "ActFinishTime", DataSource.TABLE, "", false)]
        public virtual DateTime? ActFinishTime
        {
            get
            {
                return _ActFinishTime;
            }
            set
            {
                bool isModify = false;
                if (_ActFinishTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ActFinishTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ActFinishTime") == -1)
                    {
                        UPDATE_COLUMN.Add("ActFinishTime");
                    }
                    _ActFinishTime = value;
                }
            }
        }
        
        protected string _DelayReason = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DelayReason", "DelayReason", DataSource.TABLE, "", false)]
        public virtual string DelayReason
        {
            get
            {
                return _DelayReason;
            }
            set
            {
                bool isModify = false;
                if (_DelayReason == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_DelayReason.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DelayReason") == -1)
                    {
                        UPDATE_COLUMN.Add("DelayReason");
                    }
                    _DelayReason = value;
                }
            }
        }
        
        protected string _ImprovedWay = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ImprovedWay", "ImprovedWay", DataSource.TABLE, "", false)]
        public virtual string ImprovedWay
        {
            get
            {
                return _ImprovedWay;
            }
            set
            {
                bool isModify = false;
                if (_ImprovedWay == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ImprovedWay.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ImprovedWay") == -1)
                    {
                        UPDATE_COLUMN.Add("ImprovedWay");
                    }
                    _ImprovedWay = value;
                }
            }
        }
        
        protected int? _IsFinish = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsFinish", "IsFinish", DataSource.TABLE, "", false)]
        public virtual int? IsFinish
        {
            get
            {
                return _IsFinish;
            }
            set
            {
                bool isModify = false;
                if (_IsFinish == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsFinish.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsFinish") == -1)
                    {
                        UPDATE_COLUMN.Add("IsFinish");
                    }
                    _IsFinish = value;
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
                if (_CreatedAt == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CreatedAt.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreatedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("CreatedAt");
                    }
                    _CreatedAt = value;
                }
            }
        }
        
        protected DateTime? _UpdatedAt = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UpdatedAt", "UpdatedAt", DataSource.TABLE, "", false)]
        public virtual DateTime? UpdatedAt
        {
            get
            {
                return _UpdatedAt;
            }
            set
            {
                bool isModify = false;
                if (_UpdatedAt == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_UpdatedAt.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UpdatedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("UpdatedAt");
                    }
                    _UpdatedAt = value;
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
    public partial class OFS_SCI_WorkSch_CheckStandard : IOFS_SCI_WorkSch_CheckStandard
    {
    }
    
}