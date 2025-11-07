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
    /// 科專類-階段審核人員 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_StageExam_ReviewerList", "科專類-階段審核人員", false)]
    public class IOFS_SCI_StageExam_ReviewerList : IMeta
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
        
        protected int? _ExamID = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExamID", "ExamID", DataSource.TABLE, "", false)]
        public virtual int? ExamID
        {
            get
            {
                return _ExamID;
            }
            set
            {
                bool isModify = false;
                if (_ExamID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ExamID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ExamID") == -1)
                    {
                        UPDATE_COLUMN.Add("ExamID");
                    }
                    _ExamID = value;
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
        
        protected string _ReviewFilePath = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewFilePath", "ReviewFilePath", DataSource.TABLE, "", false)]
        public virtual string ReviewFilePath
        {
            get
            {
                return _ReviewFilePath;
            }
            set
            {
                bool isModify = false;
                if (_ReviewFilePath == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewFilePath.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewFilePath") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewFilePath");
                    }
                    _ReviewFilePath = value;
                }
            }
        }
        
        protected string _token = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("token", "token", DataSource.TABLE, "", false)]
        public virtual string token
        {
            get
            {
                return _token;
            }
            set
            {
                bool isModify = false;
                if (_token == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_token.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("token") == -1)
                    {
                        UPDATE_COLUMN.Add("token");
                    }
                    _token = value;
                }
            }
        }
        
        protected bool? _isSubmit = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("isSubmit", "isSubmit", DataSource.TABLE, "", false)]
        public virtual bool? isSubmit
        {
            get
            {
                return _isSubmit;
            }
            set
            {
                bool isModify = false;
                if (_isSubmit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_isSubmit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("isSubmit") == -1)
                    {
                        UPDATE_COLUMN.Add("isSubmit");
                    }
                    _isSubmit = value;
                }
            }
        }
        
        protected string _BankCode = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("BankCode", "BankCode", DataSource.TABLE, "", false)]
        public virtual string BankCode
        {
            get
            {
                return _BankCode;
            }
            set
            {
                bool isModify = false;
                if (_BankCode == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_BankCode.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("BankCode") == -1)
                    {
                        UPDATE_COLUMN.Add("BankCode");
                    }
                    _BankCode = value;
                }
            }
        }
        
        protected string _BankAccount = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("BankAccount", "BankAccount", DataSource.TABLE, "", false)]
        public virtual string BankAccount
        {
            get
            {
                return _BankAccount;
            }
            set
            {
                bool isModify = false;
                if (_BankAccount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_BankAccount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("BankAccount") == -1)
                    {
                        UPDATE_COLUMN.Add("BankAccount");
                    }
                    _BankAccount = value;
                }
            }
        }
        
        protected string _BankBookPath = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("BankBookPath", "BankBookPath", DataSource.TABLE, "", false)]
        public virtual string BankBookPath
        {
            get
            {
                return _BankBookPath;
            }
            set
            {
                bool isModify = false;
                if (_BankBookPath == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_BankBookPath.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("BankBookPath") == -1)
                    {
                        UPDATE_COLUMN.Add("BankBookPath");
                    }
                    _BankBookPath = value;
                }
            }
        }
        
        protected string _RegistrationAddress = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RegistrationAddress", "RegistrationAddress", DataSource.TABLE, "", false)]
        public virtual string RegistrationAddress
        {
            get
            {
                return _RegistrationAddress;
            }
            set
            {
                bool isModify = false;
                if (_RegistrationAddress == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_RegistrationAddress.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("RegistrationAddress") == -1)
                    {
                        UPDATE_COLUMN.Add("RegistrationAddress");
                    }
                    _RegistrationAddress = value;
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
    /// 科專類-階段審核人員 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_StageExam_ReviewerList : IOFS_SCI_StageExam_ReviewerList
    {
    }
    
}