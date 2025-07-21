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
    /// 科專-專案版本對應表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_Project_Main", "科專-專案版本對應表", false)]
    public class IOFS_SCI_Project_Main : IMeta
    {
        
        protected string _ProjectID = "";
        ///<summary>
        /// 計畫ID (計畫ID)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProjectID", "ProjectID", DataSource.TABLE, "計畫ID", true)]
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
        /// 目前此計畫狀態 (目前此計畫狀態)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Statuses", "Statuses", DataSource.TABLE, "目前此計畫狀態", false)]
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
        /// 階段名稱 (階段名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StatusesName", "StatusesName", DataSource.TABLE, "階段名稱", false)]
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
        /// 補件逾期日期 (補件逾期日期)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExpirationDate", "ExpirationDate", DataSource.TABLE, "補件逾期日期", false)]
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
        
        protected decimal? _SeqPoint = null;
        ///<summary>
        /// 評審評分 (評審評分)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SeqPoint", "SeqPoint", DataSource.TABLE, "評審評分", false)]
        public virtual decimal? SeqPoint
        {
            get
            {
                return _SeqPoint;
            }
            set
            {
                bool isModify = false;
                if (_SeqPoint == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SeqPoint.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SeqPoint") == -1)
                    {
                        UPDATE_COLUMN.Add("SeqPoint");
                    }
                    _SeqPoint = value;
                }
            }
        }
        
        protected string _SupervisoryUnit = "";
        ///<summary>
        /// 主管審核單位 (主管審核單位)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SupervisoryUnit", "SupervisoryUnit", DataSource.TABLE, "主管審核單位", false)]
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
        
        protected string _SupervisoryPersonName = "";
        ///<summary>
        /// 主管審核人員姓名 (主管審核人員姓名)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SupervisoryPersonName", "SupervisoryPersonName", DataSource.TABLE, "主管審核人員姓名", false)]
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
        
        protected string _SupervisoryPersonAccount = "";
        ///<summary>
        /// 主管審核人員帳號 (主管審核人員帳號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SupervisoryPersonAccount", "SupervisoryPersonAccount", DataSource.TABLE, "主管審核人員帳號", false)]
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
        
        protected string _UserAccount = "";
        ///<summary>
        /// 申請者帳號 (申請者帳號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserAccount", "UserAccount", DataSource.TABLE, "申請者帳號", false)]
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
        
        protected string _UserOrg = "";
        ///<summary>
        /// 申請者單位 (申請者單位)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserOrg", "UserOrg", DataSource.TABLE, "申請者單位", false)]
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
        
        protected string _UserName = "";
        ///<summary>
        /// 申請者名稱 (申請者名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserName", "UserName", DataSource.TABLE, "申請者名稱", false)]
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
        
        protected string _Form1Status = "";
        ///<summary>
        /// 申請表單填寫狀態(申請表) (申請表單填寫狀態(申請表))
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Form1Status", "Form1Status", DataSource.TABLE, "申請表單填寫狀態(申請表)", false)]
        public virtual string Form1Status
        {
            get
            {
                return _Form1Status;
            }
            set
            {
                bool isModify = false;
                if (_Form1Status == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Form1Status.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Form1Status") == -1)
                    {
                        UPDATE_COLUMN.Add("Form1Status");
                    }
                    _Form1Status = value;
                }
            }
        }
        
        protected string _Form2Status = "";
        ///<summary>
        /// 申請表單填寫狀態(工作項目) (申請表單填寫狀態(工作項目))
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Form2Status", "Form2Status", DataSource.TABLE, "申請表單填寫狀態(工作項目)", false)]
        public virtual string Form2Status
        {
            get
            {
                return _Form2Status;
            }
            set
            {
                bool isModify = false;
                if (_Form2Status == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Form2Status.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Form2Status") == -1)
                    {
                        UPDATE_COLUMN.Add("Form2Status");
                    }
                    _Form2Status = value;
                }
            }
        }
        
        protected string _Form3Status = "";
        ///<summary>
        /// 申請表單填寫狀態(經費人事) (申請表單填寫狀態(經費人事))
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Form3Status", "Form3Status", DataSource.TABLE, "申請表單填寫狀態(經費人事)", false)]
        public virtual string Form3Status
        {
            get
            {
                return _Form3Status;
            }
            set
            {
                bool isModify = false;
                if (_Form3Status == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Form3Status.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Form3Status") == -1)
                    {
                        UPDATE_COLUMN.Add("Form3Status");
                    }
                    _Form3Status = value;
                }
            }
        }
        
        protected string _Form4Status = "";
        ///<summary>
        /// 申請表單填寫狀態(成果與績效) (申請表單填寫狀態(成果與績效))
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Form4Status", "Form4Status", DataSource.TABLE, "申請表單填寫狀態(成果與績效)", false)]
        public virtual string Form4Status
        {
            get
            {
                return _Form4Status;
            }
            set
            {
                bool isModify = false;
                if (_Form4Status == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Form4Status.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Form4Status") == -1)
                    {
                        UPDATE_COLUMN.Add("Form4Status");
                    }
                    _Form4Status = value;
                }
            }
        }
        
        protected string _Form5Status = "";
        ///<summary>
        /// 申請表單填寫狀態(其他) (申請表單填寫狀態(其他))
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Form5Status", "Form5Status", DataSource.TABLE, "申請表單填寫狀態(其他)", false)]
        public virtual string Form5Status
        {
            get
            {
                return _Form5Status;
            }
            set
            {
                bool isModify = false;
                if (_Form5Status == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Form5Status.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Form5Status") == -1)
                    {
                        UPDATE_COLUMN.Add("Form5Status");
                    }
                    _Form5Status = value;
                }
            }
        }
        protected string _CurrentStep = "";
        ///<summary>
        /// 當前狀態 (當前狀態)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CurrentStep", "CurrentStep", DataSource.TABLE, "當前狀態", false)]
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
        
        protected DateTime? _created_at = DateTime.Now;
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
        
        protected DateTime? _updated_at = DateTime.Now;
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
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專-專案版本對應表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_Project_Main : IOFS_SCI_Project_Main
    {
    }
    
}