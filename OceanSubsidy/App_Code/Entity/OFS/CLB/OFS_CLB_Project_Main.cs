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
    [GisTableAttribute("OFS_CLB_Project_Main", "", false)]
    public class IOFS_CLB_Project_Main : IMeta
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
        
        protected string _Statuses = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Statuses", "Statuses", DataSource.TABLE, "", false)]
        public virtual string Statuses
        {
            get
            {
                return _Statuses;
            }
            set
            {
                bool isModify = false;
                if (_Statuses == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Statuses.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Statuses") == -1)
                    {
                        UPDATE_COLUMN.Add("Statuses");
                    }
                    _Statuses = value;
                }
            }
        }
        
        protected string _StatusesName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StatusesName", "StatusesName", DataSource.TABLE, "", false)]
        public virtual string StatusesName
        {
            get
            {
                return _StatusesName;
            }
            set
            {
                bool isModify = false;
                if (_StatusesName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StatusesName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StatusesName") == -1)
                    {
                        UPDATE_COLUMN.Add("StatusesName");
                    }
                    _StatusesName = value;
                }
            }
        }
        
        protected DateTime? _ExpirationDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExpirationDate", "ExpirationDate", DataSource.TABLE, "", false)]
        public virtual DateTime? ExpirationDate
        {
            get
            {
                return _ExpirationDate;
            }
            set
            {
                bool isModify = false;
                if (_ExpirationDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ExpirationDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ExpirationDate") == -1)
                    {
                        UPDATE_COLUMN.Add("ExpirationDate");
                    }
                    _ExpirationDate = value;
                }
            }
        }
        
        protected string _SupervisoryPersonAccount = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SupervisoryPersonAccount", "SupervisoryPersonAccount", DataSource.TABLE, "", false)]
        public virtual string SupervisoryPersonAccount
        {
            get
            {
                return _SupervisoryPersonAccount;
            }
            set
            {
                bool isModify = false;
                if (_SupervisoryPersonAccount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SupervisoryPersonAccount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SupervisoryPersonAccount") == -1)
                    {
                        UPDATE_COLUMN.Add("SupervisoryPersonAccount");
                    }
                    _SupervisoryPersonAccount = value;
                }
            }
        }
        
        protected string _SupervisoryPersonName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SupervisoryPersonName", "SupervisoryPersonName", DataSource.TABLE, "", false)]
        public virtual string SupervisoryPersonName
        {
            get
            {
                return _SupervisoryPersonName;
            }
            set
            {
                bool isModify = false;
                if (_SupervisoryPersonName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SupervisoryPersonName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SupervisoryPersonName") == -1)
                    {
                        UPDATE_COLUMN.Add("SupervisoryPersonName");
                    }
                    _SupervisoryPersonName = value;
                }
            }
        }
        
        protected string _SupervisoryUnit = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SupervisoryUnit", "SupervisoryUnit", DataSource.TABLE, "", false)]
        public virtual string SupervisoryUnit
        {
            get
            {
                return _SupervisoryUnit;
            }
            set
            {
                bool isModify = false;
                if (_SupervisoryUnit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SupervisoryUnit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SupervisoryUnit") == -1)
                    {
                        UPDATE_COLUMN.Add("SupervisoryUnit");
                    }
                    _SupervisoryUnit = value;
                }
            }
        }
        
        protected string _UserAccount = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserAccount", "UserAccount", DataSource.TABLE, "", false)]
        public virtual string UserAccount
        {
            get
            {
                return _UserAccount;
            }
            set
            {
                bool isModify = false;
                if (_UserAccount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_UserAccount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UserAccount") == -1)
                    {
                        UPDATE_COLUMN.Add("UserAccount");
                    }
                    _UserAccount = value;
                }
            }
        }
        
        protected string _UserName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserName", "UserName", DataSource.TABLE, "", false)]
        public virtual string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                bool isModify = false;
                if (_UserName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_UserName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UserName") == -1)
                    {
                        UPDATE_COLUMN.Add("UserName");
                    }
                    _UserName = value;
                }
            }
        }
        
        protected string _UserOrg = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserOrg", "UserOrg", DataSource.TABLE, "", false)]
        public virtual string UserOrg
        {
            get
            {
                return _UserOrg;
            }
            set
            {
                bool isModify = false;
                if (_UserOrg == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_UserOrg.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UserOrg") == -1)
                    {
                        UPDATE_COLUMN.Add("UserOrg");
                    }
                    _UserOrg = value;
                }
            }
        }
        
        protected string _CurrentStep = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CurrentStep", "CurrentStep", DataSource.TABLE, "", false)]
        public virtual string CurrentStep
        {
            get
            {
                return _CurrentStep;
            }
            set
            {
                bool isModify = false;
                if (_CurrentStep == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CurrentStep.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CurrentStep") == -1)
                    {
                        UPDATE_COLUMN.Add("CurrentStep");
                    }
                    _CurrentStep = value;
                }
            }
        }
        
        protected DateTime? _created_at = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("created_at", "created_at", DataSource.TABLE, "", false)]
        public virtual DateTime? created_at
        {
            get
            {
                return _created_at;
            }
            set
            {
                bool isModify = false;
                if (_created_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_created_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("created_at") == -1)
                    {
                        UPDATE_COLUMN.Add("created_at");
                    }
                    _created_at = value;
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
        
        protected bool? _isWithdrawal = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("isWithdrawal", "isWithdrawal", DataSource.TABLE, "", false)]
        public virtual bool? isWithdrawal
        {
            get
            {
                return _isWithdrawal;
            }
            set
            {
                bool isModify = false;
                if (_isWithdrawal == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_isWithdrawal.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("isWithdrawal") == -1)
                    {
                        UPDATE_COLUMN.Add("isWithdrawal");
                    }
                    _isWithdrawal = value;
                }
            }
        }
        
        protected bool? _isExist = true;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("isExist", "isExist", DataSource.TABLE, "", false)]
        public virtual bool? isExist
        {
            get
            {
                return _isExist;
            }
            set
            {
                bool isModify = false;
                if (_isExist == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_isExist.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("isExist") == -1)
                    {
                        UPDATE_COLUMN.Add("isExist");
                    }
                    _isExist = value;
                }
            }
        }
        
        protected double? _ApprovedSubsidy = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ApprovedSubsidy", "ApprovedSubsidy", DataSource.TABLE, "", false)]
        public virtual double? ApprovedSubsidy
        {
            get
            {
                return _ApprovedSubsidy;
            }
            set
            {
                bool isModify = false;
                if (_ApprovedSubsidy == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ApprovedSubsidy.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ApprovedSubsidy") == -1)
                    {
                        UPDATE_COLUMN.Add("ApprovedSubsidy");
                    }
                    _ApprovedSubsidy = value;
                }
            }
        }
        
        protected string _FinalReviewNotes = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FinalReviewNotes", "FinalReviewNotes", DataSource.TABLE, "", false)]
        public virtual string FinalReviewNotes
        {
            get
            {
                return _FinalReviewNotes;
            }
            set
            {
                bool isModify = false;
                if (_FinalReviewNotes == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_FinalReviewNotes.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FinalReviewNotes") == -1)
                    {
                        UPDATE_COLUMN.Add("FinalReviewNotes");
                    }
                    _FinalReviewNotes = value;
                }
            }
        }
        
        protected int? _FinalReviewOrder = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FinalReviewOrder", "FinalReviewOrder", DataSource.TABLE, "", false)]
        public virtual int? FinalReviewOrder
        {
            get
            {
                return _FinalReviewOrder;
            }
            set
            {
                bool isModify = false;
                if (_FinalReviewOrder == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_FinalReviewOrder.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FinalReviewOrder") == -1)
                    {
                        UPDATE_COLUMN.Add("FinalReviewOrder");
                    }
                    _FinalReviewOrder = value;
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
    public partial class OFS_CLB_Project_Main : IOFS_CLB_Project_Main
    {
    }
    
}