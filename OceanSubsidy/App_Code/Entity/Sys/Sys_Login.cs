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
    /// 登入表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_Login", "登入表", false)]
    public class ISys_Login : IMeta
    {

        protected int _LoginID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("LoginID", "LoginID", DataSource.UN_OPERATE, "", true)]
        public virtual int LoginID
        {
            get
            {
                return _LoginID;
            }
            set
            {
                bool isModify = false;
                if (_LoginID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_LoginID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("LoginID") == -1)
                    {
                        UPDATE_COLUMN.Add("LoginID");
                    }
                    _LoginID = value;
                }
            }
        }

        protected int _UserID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserID", "UserID", DataSource.TABLE, "", false)]
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

        protected string _LoginIP = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("LoginIP", "LoginIP", DataSource.TABLE, "", false)]
        public virtual string LoginIP
        {
            get
            {
                return _LoginIP;
            }
            set
            {
                bool isModify = false;
                if (_LoginIP == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_LoginIP.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("LoginIP") == -1)
                    {
                        UPDATE_COLUMN.Add("LoginIP");
                    }
                    _LoginIP = value;
                }
            }
        }

        protected string _LoginTime = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("LoginTime", "LoginTime", DataSource.TABLE, "", false)]
        public virtual string LoginTime
        {
            get
            {
                return _LoginTime;
            }
            set
            {
                bool isModify = false;
                if (_LoginTime == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_LoginTime.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("LoginTime") == -1)
                    {
                        UPDATE_COLUMN.Add("LoginTime");
                    }
                    _LoginTime = value;
                }
            }
        }

    }




}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 登入表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_Login : ISys_Login
    {
    }

}