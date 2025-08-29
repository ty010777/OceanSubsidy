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
    [GisTableAttribute("OFS_TaskQueue", "", false)]
    public class IOFS_TaskQueue : IMeta
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
        
        protected string _TaskName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TaskName", "TaskName", DataSource.TABLE, "", false)]
        public virtual string TaskName
        {
            get
            {
                return _TaskName;
            }
            set
            {
                bool isModify = false;
                if (_TaskName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TaskName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TaskName") == -1)
                    {
                        UPDATE_COLUMN.Add("TaskName");
                    }
                    _TaskName = value;
                }
            }
        }
        
        protected int? _PriorityLevel = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PriorityLevel", "PriorityLevel", DataSource.TABLE, "", false)]
        public virtual int? PriorityLevel
        {
            get
            {
                return _PriorityLevel;
            }
            set
            {
                bool isModify = false;
                if (_PriorityLevel == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PriorityLevel.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PriorityLevel") == -1)
                    {
                        UPDATE_COLUMN.Add("PriorityLevel");
                    }
                    _PriorityLevel = value;
                }
            }
        }
        
        protected bool? _IsTodo = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsTodo", "IsTodo", DataSource.TABLE, "", false)]
        public virtual bool? IsTodo
        {
            get
            {
                return _IsTodo;
            }
            set
            {
                bool isModify = false;
                if (_IsTodo == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsTodo.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsTodo") == -1)
                    {
                        UPDATE_COLUMN.Add("IsTodo");
                    }
                    _IsTodo = value;
                }
            }
        }
        
        protected bool? _IsCompleted = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsCompleted", "IsCompleted", DataSource.TABLE, "", false)]
        public virtual bool? IsCompleted
        {
            get
            {
                return _IsCompleted;
            }
            set
            {
                bool isModify = false;
                if (_IsCompleted == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsCompleted.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsCompleted") == -1)
                    {
                        UPDATE_COLUMN.Add("IsCompleted");
                    }
                    _IsCompleted = value;
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
    public partial class OFS_TaskQueue : IOFS_TaskQueue
    {
    }
    
}