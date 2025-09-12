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
    [GisTableAttribute("OFS_CLB_StageExam", "", false)]
    public class IOFS_CLB_StageExam : IMeta
    {
        
        protected int _id = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("id", "id", DataSource.UN_OPERATE, "", false)]
        public virtual int id
        {
            get
            {
                return _id;
            }
            set
            {
                bool isModify = false;
                if (_id == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_id.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("id") == -1)
                    {
                        UPDATE_COLUMN.Add("id");
                    }
                    _id = value;
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
        
        protected string _Reviewer = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Reviewer", "Reviewer", DataSource.TABLE, "", false)]
        public virtual string Reviewer
        {
            get
            {
                return _Reviewer;
            }
            set
            {
                bool isModify = false;
                if (_Reviewer == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Reviewer.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Reviewer") == -1)
                    {
                        UPDATE_COLUMN.Add("Reviewer");
                    }
                    _Reviewer = value;
                }
            }
        }
        
        protected string _Account = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Account", "Account", DataSource.TABLE, "", false)]
        public virtual string Account
        {
            get
            {
                return _Account;
            }
            set
            {
                bool isModify = false;
                if (_Account == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Account.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Account") == -1)
                    {
                        UPDATE_COLUMN.Add("Account");
                    }
                    _Account = value;
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
    public partial class OFS_CLB_StageExam : IOFS_CLB_StageExam
    {
    }
    
}