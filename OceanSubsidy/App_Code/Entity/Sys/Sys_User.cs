using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 系統角色表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_User", "系統角色表", false)]
    public class ISys_User : IMeta
    {

        protected int _UserID = 0;
        ///<summary>
        /// PK (PK)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserID", "UserID", DataSource.UN_OPERATE, "PK", true)]
        public virtual int UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                bool isModify = false;
                if (_UserID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UserID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UserID") == -1)
                    {
                        UPDATE_COLUMN.Add("UserID");
                    }
                    _UserID = value;
                }
            }
        }

        protected int? _UnitID = null;
        ///<summary>
        /// 單位ID (單位ID)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UnitID", "UnitID", DataSource.TABLE, "單位ID", false)]
        public virtual int? UnitID
        {
            get
            {
                return _UnitID;
            }
            set
            {
                bool isModify = false;
                if (_UnitID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UnitID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UnitID") == -1)
                    {
                        UPDATE_COLUMN.Add("UnitID");
                    }
                    _UnitID = value;
                }
            }
        }

        protected string _Account = null;
        ///<summary>
        /// 帳號 (帳號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Account", "Account", DataSource.TABLE, "帳號", false)]
        public virtual string Account
        {
            get
            {
                return _Account;
            }
            set
            {
                bool isModify = false;
                if (_Account == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Account.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Account") == -1)
                    {
                        UPDATE_COLUMN.Add("Account");
                    }
                    _Account = value;
                }
            }
        }

        protected string _Pwd = null;
        ///<summary>
        /// 密碼 (密碼)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Pwd", "Pwd", DataSource.TABLE, "密碼", false)]
        public virtual string Pwd
        {
            get
            {
                return _Pwd;
            }
            set
            {
                bool isModify = false;
                if (_Pwd == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Pwd.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Pwd") == -1)
                    {
                        UPDATE_COLUMN.Add("Pwd");
                    }
                    _Pwd = value;
                }
            }
        }

        protected string _Name = null;
        ///<summary>
        /// 姓名 (姓名)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Name", "Name", DataSource.TABLE, "姓名", false)]
        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                bool isModify = false;
                if (_Name == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Name.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Name") == -1)
                    {
                        UPDATE_COLUMN.Add("Name");
                    }
                    _Name = value;
                }
            }
        }

        protected string _Tel = null;
        ///<summary>
        /// 電話 (電話)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Tel", "Tel", DataSource.TABLE, "電話", false)]
        public virtual string Tel
        {
            get
            {
                return _Tel;
            }
            set
            {
                bool isModify = false;
                if (_Tel == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Tel.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Tel") == -1)
                    {
                        UPDATE_COLUMN.Add("Tel");
                    }
                    _Tel = value;
                }
            }
        }

        protected int? _OSI_RoleID = null;
        ///<summary>
        /// 海洋科學調查角色 (海洋科學調查角色)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OSI_RoleID", "OSI_RoleID", DataSource.TABLE, "海洋科學調查角色", false)]
        public virtual int? OSI_RoleID
        {
            get
            {
                return _OSI_RoleID;
            }
            set
            {
                bool isModify = false;
                if (_OSI_RoleID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_OSI_RoleID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OSI_RoleID") == -1)
                    {
                        UPDATE_COLUMN.Add("OSI_RoleID");
                    }
                    _OSI_RoleID = value;
                }
            }
        }

        protected bool _IsReceiveMail = true;
        ///<summary>
        /// 是否收稽催信 (是否收稽催信)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsReceiveMail", "IsReceiveMail", DataSource.TABLE, "是否收稽催信", false)]
        public virtual bool IsReceiveMail
        {
            get
            {
                return _IsReceiveMail;
            }
            set
            {
                bool isModify = false;
                if (_IsReceiveMail == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_IsReceiveMail.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsReceiveMail") == -1)
                    {
                        UPDATE_COLUMN.Add("IsReceiveMail");
                    }
                    _IsReceiveMail = value;
                }
            }
        }

        protected bool _IsApproved = false;
        ///<summary>
        /// 是否通過審核 (是否通過審核)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsApproved", "IsApproved", DataSource.TABLE, "是否通過審核", false)]
        public virtual bool IsApproved
        {
            get
            {
                return _IsApproved;
            }
            set
            {
                bool isModify = false;
                if (_IsApproved == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_IsApproved.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsApproved") == -1)
                    {
                        UPDATE_COLUMN.Add("IsApproved");
                    }
                    _IsApproved = value;
                }
            }
        }

        protected bool _IsValid = false;
        ///<summary>
        /// 假刪除旗標 (假刪除旗標)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "假刪除旗標", false)]
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

        protected string _Salt = Guid.NewGuid().ToString().ToUpper();
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Salt", "Salt", DataSource.TABLE, "", false)]
        public virtual string Salt
        {
            get
            {
                return _Salt;
            }
            set
            {
                bool isModify = false;
                if (_Salt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Salt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Salt") == -1)
                    {
                        UPDATE_COLUMN.Add("Salt");
                    }
                    _Salt = value;
                }
            }
        }

        protected DateTime _CreateTime = DateTime.Now;
        ///<summary>
        /// 帳號新增時間 (帳號新增時間)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreateTime", "CreateTime", DataSource.TABLE, "帳號新增時間", false)]
        public virtual DateTime CreateTime
        {
            get
            {
                return _CreateTime;
            }
            set
            {
                bool isModify = false;
                if (_CreateTime == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CreateTime.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreateTime") == -1)
                    {
                        UPDATE_COLUMN.Add("CreateTime");
                    }
                    _CreateTime = value;
                }
            }
        }

        protected DateTime _UpdateTime = DateTime.Now;
        ///<summary>
        /// 帳號變更時間 (帳號變更時間)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UpdateTime", "UpdateTime", DataSource.TABLE, "帳號變更時間", false)]
        public virtual DateTime UpdateTime
        {
            get
            {
                return _UpdateTime;
            }
            set
            {
                bool isModify = false;
                if (_UpdateTime == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UpdateTime.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UpdateTime") == -1)
                    {
                        UPDATE_COLUMN.Add("UpdateTime");
                    }
                    _UpdateTime = value;
                }
            }
        }

        protected string _PwdToken = null;
        ///<summary>
        /// 密碼變更時的token (密碼變更時的token)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PwdToken", "PwdToken", DataSource.TABLE, "密碼變更時的token", false)]
        public virtual string PwdToken
        {
            get
            {
                return _PwdToken;
            }
            set
            {
                bool isModify = false;
                if (_PwdToken == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_PwdToken.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PwdToken") == -1)
                    {
                        UPDATE_COLUMN.Add("PwdToken");
                    }
                    _PwdToken = value;
                }
            }
        }       

        protected bool _IsActive = true;
        ///<summary>
        /// 啟用狀態 (啟用狀態)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsActive", "IsActive", DataSource.TABLE, "啟用狀態", false)]
        public virtual bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                bool isModify = false;
                if (_IsActive == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_IsActive.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsActive") == -1)
                    {
                        UPDATE_COLUMN.Add("IsActive");
                    }
                    _IsActive = value;
                }
            }
        }

        protected string _UnitName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UnitName", "UnitName", DataSource.TABLE, "", false)]
        public virtual string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                bool isModify = false;
                if (_UnitName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UnitName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UnitName") == -1)
                    {
                        UPDATE_COLUMN.Add("UnitName");
                    }
                    _UnitName = value;
                }
            }
        }


        protected int _UnitType = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UnitType", "UnitType", DataSource.TABLE, "", false)]
        public virtual int UnitType
        {
            get
            {
                return _UnitType;
            }
            set
            {
                bool isModify = false;
                if (_UnitType == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UnitType.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UnitType") == -1)
                    {
                        UPDATE_COLUMN.Add("UnitType");
                    }
                    _UnitType = value;
                }
            }
        }

        protected string _ApprovedSource = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ApprovedSource", "ApprovedSource", DataSource.TABLE, "", false)]
        public virtual string ApprovedSource
        {
            get
            {
                return _ApprovedSource;
            }
            set
            {
                bool isModify = false;
                if (_ApprovedSource == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ApprovedSource.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ApprovedSource") == -1)
                    {
                        UPDATE_COLUMN.Add("ApprovedSource");
                    }
                    _ApprovedSource = value;
                }
            }
        }


    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 系統角色表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_User : ISys_User
    {
    }
}