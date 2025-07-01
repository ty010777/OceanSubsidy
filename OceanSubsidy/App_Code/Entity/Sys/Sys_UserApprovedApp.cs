using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 帳號與申請系統對照表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("Sys_UserApprovedApp", "帳號與申請系統對照表", false)]
    public class ISys_UserApprovedApp : IMeta
    {

        protected int _UserAppID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UserAppID", "UserAppID", DataSource.UN_OPERATE, "", true)]
        public virtual int UserAppID
        {
            get
            {
                return _UserAppID;
            }
            set
            {
                bool isModify = false;
                if (_UserAppID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UserAppID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UserAppID") == -1)
                    {
                        UPDATE_COLUMN.Add("UserAppID");
                    }
                    _UserAppID = value;
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

        protected int _SystemID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SystemID", "SystemID", DataSource.TABLE, "", false)]
        public virtual int SystemID
        {
            get
            {
                return _SystemID;
            }
            set
            {
                bool isModify = false;
                if (_SystemID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_SystemID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SystemID") == -1)
                    {
                        UPDATE_COLUMN.Add("SystemID");
                    }
                    _SystemID = value;
                }
            }
        }

    }

}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;


    /// <summary>
    /// 帳號與申請系統對照表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class Sys_UserApprovedApp : ISys_UserApprovedApp
    {
    }

}
