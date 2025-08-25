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
    [GisTableAttribute("OFS_GrantTargetSetting", "", false)]
    public class IOFS_GrantTargetSetting : IMeta
    {
        
        protected int _ID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ID", "ID", DataSource.UN_OPERATE, "", false)]
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
        
        protected string _GrantTypeID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("GrantTypeID", "GrantTypeID", DataSource.TABLE, "", false)]
        public virtual string GrantTypeID
        {
            get
            {
                return _GrantTypeID;
            }
            set
            {
                bool isModify = false;
                if (_GrantTypeID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_GrantTypeID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("GrantTypeID") == -1)
                    {
                        UPDATE_COLUMN.Add("GrantTypeID");
                    }
                    _GrantTypeID = value;
                }
            }
        }
        
        protected string _TargetTypeID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TargetTypeID", "TargetTypeID", DataSource.TABLE, "", false)]
        public virtual string TargetTypeID
        {
            get
            {
                return _TargetTypeID;
            }
            set
            {
                bool isModify = false;
                if (_TargetTypeID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TargetTypeID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TargetTypeID") == -1)
                    {
                        UPDATE_COLUMN.Add("TargetTypeID");
                    }
                    _TargetTypeID = value;
                }
            }
        }
        
        protected string _TargetName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TargetName", "TargetName", DataSource.TABLE, "", false)]
        public virtual string TargetName
        {
            get
            {
                return _TargetName;
            }
            set
            {
                bool isModify = false;
                if (_TargetName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TargetName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TargetName") == -1)
                    {
                        UPDATE_COLUMN.Add("TargetName");
                    }
                    _TargetName = value;
                }
            }
        }
        
        protected decimal? _MatchingFund = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("MatchingFund", "MatchingFund", DataSource.TABLE, "", false)]
        public virtual decimal? MatchingFund
        {
            get
            {
                return _MatchingFund;
            }
            set
            {
                bool isModify = false;
                if (_MatchingFund == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_MatchingFund.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("MatchingFund") == -1)
                    {
                        UPDATE_COLUMN.Add("MatchingFund");
                    }
                    _MatchingFund = value;
                }
            }
        }
        
        protected decimal? _GrantLimit = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("GrantLimit", "GrantLimit", DataSource.TABLE, "", false)]
        public virtual decimal? GrantLimit
        {
            get
            {
                return _GrantLimit;
            }
            set
            {
                bool isModify = false;
                if (_GrantLimit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_GrantLimit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("GrantLimit") == -1)
                    {
                        UPDATE_COLUMN.Add("GrantLimit");
                    }
                    _GrantLimit = value;
                }
            }
        }
        
        protected string _Note = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Note", "Note", DataSource.TABLE, "", false)]
        public virtual string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                bool isModify = false;
                if (_Note == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Note.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Note") == -1)
                    {
                        UPDATE_COLUMN.Add("Note");
                    }
                    _Note = value;
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
    public partial class OFS_GrantTargetSetting : IOFS_GrantTargetSetting
    {
    }
    
}