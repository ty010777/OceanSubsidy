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
    [GisTableAttribute("OFS_CaseHistoryLog", "", false)]
    public class IOFS_CaseHistoryLog : IMeta
    {
        
        protected int _Id = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Id", "Id", DataSource.UN_OPERATE, "", true)]
        public virtual int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                bool isModify = false;
                if (_Id == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Id.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Id") == -1)
                    {
                        UPDATE_COLUMN.Add("Id");
                    }
                    _Id = value;
                }
            }
        }
        
        protected string _ProjectID = null;
        ///<summary>
        /// 計畫ID
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
        
        protected DateTime _ChangeTime = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ChangeTime", "ChangeTime", DataSource.TABLE, "", false)]
        public virtual DateTime ChangeTime
        {
            get
            {
                return _ChangeTime;
            }
            set
            {
                bool isModify = false;
                if (_ChangeTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ChangeTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ChangeTime") == -1)
                    {
                        UPDATE_COLUMN.Add("ChangeTime");
                    }
                    _ChangeTime = value;
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
        
        protected string _StageStatusBefore = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StageStatusBefore", "StageStatusBefore", DataSource.TABLE, "", false)]
        public virtual string StageStatusBefore
        {
            get
            {
                return _StageStatusBefore;
            }
            set
            {
                bool isModify = false;
                if (_StageStatusBefore == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StageStatusBefore.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StageStatusBefore") == -1)
                    {
                        UPDATE_COLUMN.Add("StageStatusBefore");
                    }
                    _StageStatusBefore = value;
                }
            }
        }
        
        protected string _StageStatusAfter = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StageStatusAfter", "StageStatusAfter", DataSource.TABLE, "", false)]
        public virtual string StageStatusAfter
        {
            get
            {
                return _StageStatusAfter;
            }
            set
            {
                bool isModify = false;
                if (_StageStatusAfter == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StageStatusAfter.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StageStatusAfter") == -1)
                    {
                        UPDATE_COLUMN.Add("StageStatusAfter");
                    }
                    _StageStatusAfter = value;
                }
            }
        }
        
        protected string _Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Description", "Description", DataSource.TABLE, "", false)]
        public virtual string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                bool isModify = false;
                if (_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Description");
                    }
                    _Description = value;
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
    public partial class OFS_CaseHistoryLog : IOFS_CaseHistoryLog
    {
    }
    
}