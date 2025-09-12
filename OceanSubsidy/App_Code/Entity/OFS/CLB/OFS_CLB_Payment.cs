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
    [GisTableAttribute("OFS_CLB_Payment", "", false)]
    public class IOFS_CLB_Payment : IMeta
    {
        
        protected int _ID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ID", "ID", DataSource.UN_OPERATE, "", false)]
        public virtual int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                bool isModify = false;
                if (_ID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ID") == -1)
                    {
                        UPDATE_COLUMN.Add("ID");
                    }
                    _ID = value;
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
        
        protected int? _Stage = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Stage", "Stage", DataSource.TABLE, "", false)]
        public virtual int? Stage
        {
            get
            {
                return _Stage;
            }
            set
            {
                bool isModify = false;
                if (_Stage == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Stage.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Stage") == -1)
                    {
                        UPDATE_COLUMN.Add("Stage");
                    }
                    _Stage = value;
                }
            }
        }
        
        protected decimal? _CurrentRequestAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CurrentRequestAmount", "CurrentRequestAmount", DataSource.TABLE, "", false)]
        public virtual decimal? CurrentRequestAmount
        {
            get
            {
                return _CurrentRequestAmount;
            }
            set
            {
                bool isModify = false;
                if (_CurrentRequestAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CurrentRequestAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CurrentRequestAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("CurrentRequestAmount");
                    }
                    _CurrentRequestAmount = value;
                }
            }
        }
        
        protected decimal? _TotalSpentAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TotalSpentAmount", "TotalSpentAmount", DataSource.TABLE, "", false)]
        public virtual decimal? TotalSpentAmount
        {
            get
            {
                return _TotalSpentAmount;
            }
            set
            {
                bool isModify = false;
                if (_TotalSpentAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TotalSpentAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TotalSpentAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("TotalSpentAmount");
                    }
                    _TotalSpentAmount = value;
                }
            }
        }
        
        protected decimal? _CurrentActualPaidAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CurrentActualPaidAmount", "CurrentActualPaidAmount", DataSource.TABLE, "", false)]
        public virtual decimal? CurrentActualPaidAmount
        {
            get
            {
                return _CurrentActualPaidAmount;
            }
            set
            {
                bool isModify = false;
                if (_CurrentActualPaidAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CurrentActualPaidAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CurrentActualPaidAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("CurrentActualPaidAmount");
                    }
                    _CurrentActualPaidAmount = value;
                }
            }
        }
        
        protected string _Status = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Status", "Status", DataSource.TABLE, "", false)]
        public virtual string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                bool isModify = false;
                if (_Status == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Status.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Status") == -1)
                    {
                        UPDATE_COLUMN.Add("Status");
                    }
                    _Status = value;
                }
            }
        }
        
        protected string _ReviewerComment = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewerComment", "ReviewerComment", DataSource.TABLE, "", false)]
        public virtual string ReviewerComment
        {
            get
            {
                return _ReviewerComment;
            }
            set
            {
                bool isModify = false;
                if (_ReviewerComment == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewerComment.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewerComment") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewerComment");
                    }
                    _ReviewerComment = value;
                }
            }
        }
        
        protected string _ReviewUser = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewUser", "ReviewUser", DataSource.TABLE, "", false)]
        public virtual string ReviewUser
        {
            get
            {
                return _ReviewUser;
            }
            set
            {
                bool isModify = false;
                if (_ReviewUser == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewUser.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewUser") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewUser");
                    }
                    _ReviewUser = value;
                }
            }
        }
        
        protected DateTime? _ReviewTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewTime", "ReviewTime", DataSource.TABLE, "", false)]
        public virtual DateTime? ReviewTime
        {
            get
            {
                return _ReviewTime;
            }
            set
            {
                bool isModify = false;
                if (_ReviewTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewTime") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewTime");
                    }
                    _ReviewTime = value;
                }
            }
        }
        
        protected DateTime? _CreateTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreateTime", "CreateTime", DataSource.TABLE, "", false)]
        public virtual DateTime? CreateTime
        {
            get
            {
                return _CreateTime;
            }
            set
            {
                bool isModify = false;
                if (_CreateTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CreateTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreateTime") == -1)
                    {
                        UPDATE_COLUMN.Add("CreateTime");
                    }
                    _CreateTime = value;
                }
            }
        }
        
        protected DateTime? _UpdateTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UpdateTime", "UpdateTime", DataSource.TABLE, "", false)]
        public virtual DateTime? UpdateTime
        {
            get
            {
                return _UpdateTime;
            }
            set
            {
                bool isModify = false;
                if (_UpdateTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_UpdateTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UpdateTime") == -1)
                    {
                        UPDATE_COLUMN.Add("UpdateTime");
                    }
                    _UpdateTime = value;
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
    public partial class OFS_CLB_Payment : IOFS_CLB_Payment
    {
    }
    
}