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
    [GisTableAttribute("OFS_ReviewCommitteeList", "", false)]
    public class IOFS_ReviewCommitteeList : IMeta
    {
        
        protected int _ID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ID", "ID", DataSource.UN_OPERATE, "", true)]
        public virtual int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                bool isModify = false;
                if (_ID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ID") == -1)
                    {
                        UPDATE_COLUMN.Add("ID");
                    }
                    _ID = value;
                }
            }
        }
        
        protected string _CommitteeUser = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CommitteeUser", "CommitteeUser", DataSource.TABLE, "", false)]
        public virtual string CommitteeUser
        {
            get
            {
                return _CommitteeUser;
            }
            set
            {
                bool isModify = false;
                if (_CommitteeUser == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CommitteeUser.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CommitteeUser") == -1)
                    {
                        UPDATE_COLUMN.Add("CommitteeUser");
                    }
                    _CommitteeUser = value;
                }
            }
        }
        
        protected string _Email = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Email", "Email", DataSource.TABLE, "", false)]
        public virtual string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                bool isModify = false;
                if (_Email == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Email.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Email") == -1)
                    {
                        UPDATE_COLUMN.Add("Email");
                    }
                    _Email = value;
                }
            }
        }
        
        protected string _SubjectTypeID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SubjectTypeID", "SubjectTypeID", DataSource.TABLE, "", false)]
        public virtual string SubjectTypeID
        {
            get
            {
                return _SubjectTypeID;
            }
            set
            {
                bool isModify = false;
                if (_SubjectTypeID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SubjectTypeID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SubjectTypeID") == -1)
                    {
                        UPDATE_COLUMN.Add("SubjectTypeID");
                    }
                    _SubjectTypeID = value;
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
    public partial class OFS_ReviewCommitteeList : IOFS_ReviewCommitteeList
    {
    }
    
}