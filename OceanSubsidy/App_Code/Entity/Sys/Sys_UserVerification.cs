using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{

    /// <summary>
    /// 申請帳號用的驗證碼 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_UserVerification", "申請帳號用的驗證碼", false)]
    public class ISys_UserVerification : IMeta
    {

        protected int _ID = 0;
        ///<summary>
        /// PK (PK)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ID", "ID", DataSource.UN_OPERATE, "PK", true)]
        public virtual int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                bool isModify = false;
                if (_ID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ID") == -1)
                    {
                        UPDATE_COLUMN.Add("ID");
                    }
                    _ID = value;
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

        protected string _VerificationCode = "";
        ///<summary>
        /// 驗證碼 (驗證碼)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("VerificationCode", "VerificationCode", DataSource.TABLE, "驗證碼", false)]
        public virtual string VerificationCode
        {
            get
            {
                return _VerificationCode;
            }
            set
            {
                bool isModify = false;
                if (_VerificationCode == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_VerificationCode.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("VerificationCode") == -1)
                    {
                        UPDATE_COLUMN.Add("VerificationCode");
                    }
                    _VerificationCode = value;
                }
            }
        }

        protected DateTime _UpdateTime = DateTime.Now;
        ///<summary>
        /// 更新時間 (更新時間)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UpdateTime", "UpdateTime", DataSource.TABLE, "更新時間", false)]
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

    }



}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 申請帳號用的驗證碼 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_UserVerification : ISys_UserVerification
    {
    }


}