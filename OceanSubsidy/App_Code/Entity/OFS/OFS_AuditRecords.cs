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
    [GisTableAttribute("OFS_AuditRecords", "", false)]
    public class IOFS_AuditRecords : IMeta
    {

        protected int _idx = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("idx", "idx", DataSource.UN_OPERATE, "", false)]
        public virtual int idx
        {
            get
            {
                return _idx;
            }
            set
            {
                bool isModify = false;
                if (_idx == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_idx.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("idx") == -1)
                    {
                        UPDATE_COLUMN.Add("idx");
                    }
                    _idx = value;
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

        protected DateTime? _CheckDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CheckDate", "CheckDate", DataSource.TABLE, "", false)]
        public virtual DateTime? CheckDate
        {
            get
            {
                return _CheckDate;
            }
            set
            {
                bool isModify = false;
                if (_CheckDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CheckDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CheckDate") == -1)
                    {
                        UPDATE_COLUMN.Add("CheckDate");
                    }
                    _CheckDate = value;
                }
            }
        }

        protected string _Risk = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Risk", "Risk", DataSource.TABLE, "", false)]
        public virtual string Risk
        {
            get
            {
                return _Risk;
            }
            set
            {
                bool isModify = false;
                if (_Risk == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Risk.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Risk") == -1)
                    {
                        UPDATE_COLUMN.Add("Risk");
                    }
                    _Risk = value;
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

        protected string _ExecutorComment = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExecutorComment", "ExecutorComment", DataSource.TABLE, "", false)]
        public virtual string ExecutorComment
        {
            get
            {
                return _ExecutorComment;
            }
            set
            {
                bool isModify = false;
                if (_ExecutorComment == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ExecutorComment.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ExecutorComment") == -1)
                    {
                        UPDATE_COLUMN.Add("ExecutorComment");
                    }
                    _ExecutorComment = value;
                }
            }
        }

        protected DateTime? _create_at = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("create_at", "create_at", DataSource.TABLE, "", false)]
        public virtual DateTime? create_at
        {
            get
            {
                return _create_at;
            }
            set
            {
                bool isModify = false;
                if (_create_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_create_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("create_at") == -1)
                    {
                        UPDATE_COLUMN.Add("create_at");
                    }
                    _create_at = value;
                }
            }
        }

        protected DateTime? _update_at = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("update_at", "update_at", DataSource.TABLE, "", false)]
        public virtual DateTime? update_at
        {
            get
            {
                return _update_at;
            }
            set
            {
                bool isModify = false;
                if (_update_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_update_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("update_at") == -1)
                    {
                        UPDATE_COLUMN.Add("update_at");
                    }
                    _update_at = value;
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
    public partial class OFS_AuditRecords : IOFS_AuditRecords
    {
        [DataMember]
        public string ProjectName { get; set; }
    }

}
