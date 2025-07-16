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
    /// 科專申請表-上傳附件 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_UploadFile", "科專申請表-上傳附件", false)]
    public class IOFS_SCI_UploadFile : IMeta
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
        
        protected string _ProjectID = "";
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
        
        protected string _FileCode = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FileCode", "FileCode", DataSource.TABLE, "", false)]
        public virtual string FileCode
        {
            get
            {
                return _FileCode;
            }
            set
            {
                bool isModify = false;
                if (_FileCode == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_FileCode.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FileCode") == -1)
                    {
                        UPDATE_COLUMN.Add("FileCode");
                    }
                    _FileCode = value;
                }
            }
        }
        
        protected string _FileName = "";
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
                if (_FileName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_FileName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FileName") == -1)
                    {
                        UPDATE_COLUMN.Add("FileName");
                    }
                    _FileName = value;
                }
            }
        }
        
        protected string _TemplatePath = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TemplatePath", "TemplatePath", DataSource.TABLE, "", false)]
        public virtual string TemplatePath
        {
            get
            {
                return _TemplatePath;
            }
            set
            {
                bool isModify = false;
                if (_TemplatePath == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TemplatePath.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TemplatePath") == -1)
                    {
                        UPDATE_COLUMN.Add("TemplatePath");
                    }
                    _TemplatePath = value;
                }
            }
        }
        
        protected string _Statuses = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Statuses", "Statuses", DataSource.TABLE, "", false)]
        public virtual string Statuses
        {
            get
            {
                return _Statuses;
            }
            set
            {
                bool isModify = false;
                if (_Statuses == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Statuses.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Statuses") == -1)
                    {
                        UPDATE_COLUMN.Add("Statuses");
                    }
                    _Statuses = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專申請表-上傳附件 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_UploadFile : IOFS_SCI_UploadFile
    {
    }
    
}