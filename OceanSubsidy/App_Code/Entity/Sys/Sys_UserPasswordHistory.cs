using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 使用者密碼歷史記錄表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_UserPasswordHistory", "使用者密碼歷史記錄表", false)]
    public class ISys_UserPasswordHistory : IMeta
    {

        protected int _HistoryID = 0;
        ///<summary>
        /// 歷史記錄ID (歷史記錄ID)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("HistoryID", "HistoryID", DataSource.UN_OPERATE, "歷史記錄ID", true)]
        public virtual int HistoryID
        {
            get
            {
                return _HistoryID;
            }
            set
            {
                bool isModify = false;
                if (_HistoryID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_HistoryID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("HistoryID") == -1)
                    {
                        UPDATE_COLUMN.Add("HistoryID");
                    }
                    _HistoryID = value;
                }
            }
        }

        protected int _UserID = 0;
        ///<summary>
        /// 使用者ID (使用者ID)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserID", "UserID", DataSource.TABLE, "使用者ID", false)]
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

        protected string _Pwd = null;
        ///<summary>
        /// 加密後的密碼 (加密後的密碼)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Pwd", "Pwd", DataSource.TABLE, "加密後的密碼", false)]
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

        protected string _Salt = null;
        ///<summary>
        /// 密碼Salt值 (密碼Salt值)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Salt", "Salt", DataSource.TABLE, "密碼Salt值", false)]
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

        protected DateTime _ChangeTime = DateTime.Now;
        ///<summary>
        /// 密碼變更時間 (密碼變更時間)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ChangeTime", "ChangeTime", DataSource.TABLE, "密碼變更時間", false)]
        public virtual DateTime ChangeTime
        {
            get
            {
                return _ChangeTime;
            }
            set
            {
                bool isModify = false;
                if (_ChangeTime == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ChangeTime.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ChangeTime") == -1)
                    {
                        UPDATE_COLUMN.Add("ChangeTime");
                    }
                    _ChangeTime = value;
                }
            }
        }

    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 使用者密碼歷史記錄表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_UserPasswordHistory : ISys_UserPasswordHistory
    {
    }
}