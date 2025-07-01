using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// OSI活動執行者 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_ActivityExecutors", "OSI活動執行者", false)]
    public class IOSI_ActivityExecutors : IMeta
    {

        protected int _ExecutorID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExecutorID", "ExecutorID", DataSource.UN_OPERATE, "", true)]
        public virtual int ExecutorID
        {
            get
            {
                return _ExecutorID;
            }
            set
            {
                bool isModify = false;
                if (_ExecutorID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ExecutorID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ExecutorID") == -1)
                    {
                        UPDATE_COLUMN.Add("ExecutorID");
                    }
                    _ExecutorID = value;
                }
            }
        }

        protected int _ReportID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReportID", "ReportID", DataSource.TABLE, "", false)]
        public virtual int ReportID
        {
            get
            {
                return _ReportID;
            }
            set
            {
                bool isModify = false;
                if (_ReportID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ReportID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReportID") == -1)
                    {
                        UPDATE_COLUMN.Add("ReportID");
                    }
                    _ReportID = value;
                }
            }
        }

        protected int _CategoryID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CategoryID", "CategoryID", DataSource.TABLE, "", false)]
        public virtual int CategoryID
        {
            get
            {
                return _CategoryID;
            }
            set
            {
                bool isModify = false;
                if (_CategoryID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CategoryID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CategoryID") == -1)
                    {
                        UPDATE_COLUMN.Add("CategoryID");
                    }
                    _CategoryID = value;
                }
            }
        }

        protected string _ExecutorName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExecutorName", "ExecutorName", DataSource.TABLE, "", false)]
        public virtual string ExecutorName
        {
            get
            {
                return _ExecutorName;
            }
            set
            {
                bool isModify = false;
                if (_ExecutorName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ExecutorName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ExecutorName") == -1)
                    {
                        UPDATE_COLUMN.Add("ExecutorName");
                    }
                    _ExecutorName = value;
                }
            }
        }

        protected bool _IsValid = true;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "", false)]
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

        protected DateTime _CreatedAt = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreatedAt", "CreatedAt", DataSource.TABLE, "", false)]
        public virtual DateTime CreatedAt
        {
            get
            {
                return _CreatedAt;
            }
            set
            {
                bool isModify = false;
                if (_CreatedAt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CreatedAt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreatedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("CreatedAt");
                    }
                    _CreatedAt = value;
                }
            }
        }

        protected DateTime? _DeletedAt = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DeletedAt", "DeletedAt", DataSource.TABLE, "", false)]
        public virtual DateTime? DeletedAt
        {
            get
            {
                return _DeletedAt;
            }
            set
            {
                bool isModify = false;
                if (_DeletedAt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_DeletedAt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DeletedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("DeletedAt");
                    }
                    _DeletedAt = value;
                }
            }
        }

        protected string _DeletedBy = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DeletedBy", "DeletedBy", DataSource.TABLE, "", false)]
        public virtual string DeletedBy
        {
            get
            {
                return _DeletedBy;
            }
            set
            {
                bool isModify = false;
                if (_DeletedBy == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_DeletedBy.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DeletedBy") == -1)
                    {
                        UPDATE_COLUMN.Add("DeletedBy");
                    }
                    _DeletedBy = value;
                }
            }
        }




    }

}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;


    /// <summary>
    /// OSI活動執行者 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_ActivityExecutors : IOSI_ActivityExecutors
    {
    }
}
