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
    [GisTableAttribute("OFS_ReviewRecords", "", false)]
    public class IOFS_ReviewRecords : IMeta
    {
        
        protected int _ReviewID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewID", "ReviewID", DataSource.UN_OPERATE, "", true)]
        public virtual int ReviewID
        {
            get
            {
                return _ReviewID;
            }
            set
            {
                bool isModify = false;
                if (_ReviewID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewID") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewID");
                    }
                    _ReviewID = value;
                }
            }
        }
        
        protected string _ProjectID = null;
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
        
        protected string _ReviewStage = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewStage", "ReviewStage", DataSource.TABLE, "", false)]
        public virtual string ReviewStage
        {
            get
            {
                return _ReviewStage;
            }
            set
            {
                bool isModify = false;
                if (_ReviewStage == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewStage.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewStage") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewStage");
                    }
                    _ReviewStage = value;
                }
            }
        }
        
        protected string _Email = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Email", "Email", DataSource.TABLE, "", false)]
        public virtual string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                bool isModify = false;
                if (_Email == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Email.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Email") == -1)
                    {
                        UPDATE_COLUMN.Add("Email");
                    }
                    _Email = value;
                }
            }
        }
        
        protected string _ReviewerName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewerName", "ReviewerName", DataSource.TABLE, "", false)]
        public virtual string ReviewerName
        {
            get
            {
                return _ReviewerName;
            }
            set
            {
                bool isModify = false;
                if (_ReviewerName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewerName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewerName") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewerName");
                    }
                    _ReviewerName = value;
                }
            }
        }
        
        protected string _ReviewComment = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewComment", "ReviewComment", DataSource.TABLE, "", false)]
        public virtual string ReviewComment
        {
            get
            {
                return _ReviewComment;
            }
            set
            {
                bool isModify = false;
                if (_ReviewComment == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewComment.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewComment") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewComment");
                    }
                    _ReviewComment = value;
                }
            }
        }
        
        protected string _ReplyComment = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReplyComment", "ReplyComment", DataSource.TABLE, "", false)]
        public virtual string ReplyComment
        {
            get
            {
                return _ReplyComment;
            }
            set
            {
                bool isModify = false;
                if (_ReplyComment == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReplyComment.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReplyComment") == -1)
                    {
                        UPDATE_COLUMN.Add("ReplyComment");
                    }
                    _ReplyComment = value;
                }
            }
        }
        
        protected decimal? _TotalScore = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TotalScore", "TotalScore", DataSource.TABLE, "", false)]
        public virtual decimal? TotalScore
        {
            get
            {
                return _TotalScore;
            }
            set
            {
                bool isModify = false;
                if (_TotalScore == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TotalScore.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TotalScore") == -1)
                    {
                        UPDATE_COLUMN.Add("TotalScore");
                    }
                    _TotalScore = value;
                }
            }
        }
        
        protected string _Token = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Token", "Token", DataSource.TABLE, "", false)]
        public virtual string Token
        {
            get
            {
                return _Token;
            }
            set
            {
                bool isModify = false;
                if (_Token == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Token.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Token") == -1)
                    {
                        UPDATE_COLUMN.Add("Token");
                    }
                    _Token = value;
                }
            }
        }
        
        protected bool? _IsSubmit = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsSubmit", "IsSubmit", DataSource.TABLE, "", false)]
        public virtual bool? IsSubmit
        {
            get
            {
                return _IsSubmit;
            }
            set
            {
                bool isModify = false;
                if (_IsSubmit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsSubmit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsSubmit") == -1)
                    {
                        UPDATE_COLUMN.Add("IsSubmit");
                    }
                    _IsSubmit = value;
                }
            }
        }
        
        protected DateTime? _CreateTime = DateTime.Now;
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
        
        protected DateTime? _updated_at = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("updated_at", "updated_at", DataSource.TABLE, "", false)]
        public virtual DateTime? updated_at
        {
            get
            {
                return _updated_at;
            }
            set
            {
                bool isModify = false;
                if (_updated_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_updated_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("updated_at") == -1)
                    {
                        UPDATE_COLUMN.Add("updated_at");
                    }
                    _updated_at = value;
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
    public partial class OFS_ReviewRecords : IOFS_ReviewRecords
    {
    }
    
}