using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 活動附件表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_ActivityFiles", "活動附件表", false)]
    public class IOSI_ActivityFiles : IMeta
    {

        protected int _AttachmentID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AttachmentID", "AttachmentID", DataSource.UN_OPERATE, "", true)]
        public virtual int AttachmentID
        {
            get
            {
                return _AttachmentID;
            }
            set
            {
                bool isModify = false;
                if (_AttachmentID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AttachmentID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AttachmentID") == -1)
                    {
                        UPDATE_COLUMN.Add("AttachmentID");
                    }
                    _AttachmentID = value;
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

        protected string _FileName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FileName", "FileName", DataSource.TABLE, "", false)]
        public virtual string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                bool isModify = false;
                if (_FileName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_FileName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FileName") == -1)
                    {
                        UPDATE_COLUMN.Add("FileName");
                    }
                    _FileName = value;
                }
            }
        }

        protected string _FilePath = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FilePath", "FilePath", DataSource.TABLE, "", false)]
        public virtual string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                bool isModify = false;
                if (_FilePath == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_FilePath.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FilePath") == -1)
                    {
                        UPDATE_COLUMN.Add("FilePath");
                    }
                    _FilePath = value;
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
    /// 活動附件表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_ActivityFiles : IOSI_ActivityFiles
    {
    }

}