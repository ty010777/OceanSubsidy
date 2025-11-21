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
    /// 審查標準範本 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_ReviewTemplate", "審查標準範本", false)]
    public class IOFS_ReviewTemplate : IMeta
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
        
        protected string _SubsidyProjects = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SubsidyProjects", "SubsidyProjects", DataSource.TABLE, "", false)]
        public virtual string SubsidyProjects
        {
            get
            {
                return _SubsidyProjects;
            }
            set
            {
                bool isModify = false;
                if (_SubsidyProjects == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SubsidyProjects.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SubsidyProjects") == -1)
                    {
                        UPDATE_COLUMN.Add("SubsidyProjects");
                    }
                    _SubsidyProjects = value;
                }
            }
        }
        
        protected string _TemplateName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TemplateName", "TemplateName", DataSource.TABLE, "", false)]
        public virtual string TemplateName
        {
            get
            {
                return _TemplateName;
            }
            set
            {
                bool isModify = false;
                if (_TemplateName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TemplateName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TemplateName") == -1)
                    {
                        UPDATE_COLUMN.Add("TemplateName");
                    }
                    _TemplateName = value;
                }
            }
        }
        
        protected decimal? _TemplateWeight = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TemplateWeight", "TemplateWeight", DataSource.TABLE, "", false)]
        public virtual decimal? TemplateWeight
        {
            get
            {
                return _TemplateWeight;
            }
            set
            {
                bool isModify = false;
                if (_TemplateWeight == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TemplateWeight.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TemplateWeight") == -1)
                    {
                        UPDATE_COLUMN.Add("TemplateWeight");
                    }
                    _TemplateWeight = value;
                }
            }
        }
        
        protected string _TemplateNote = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TemplateNote", "TemplateNote", DataSource.TABLE, "", false)]
        public virtual string TemplateNote
        {
            get
            {
                return _TemplateNote;
            }
            set
            {
                bool isModify = false;
                if (_TemplateNote == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TemplateNote.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TemplateNote") == -1)
                    {
                        UPDATE_COLUMN.Add("TemplateNote");
                    }
                    _TemplateNote = value;
                }
            }
        }
        
        protected int? _OrderNo = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OrderNo", "OrderNo", DataSource.TABLE, "", false)]
        public virtual int? OrderNo
        {
            get
            {
                return _OrderNo;
            }
            set
            {
                bool isModify = false;
                if (_OrderNo == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_OrderNo.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OrderNo") == -1)
                    {
                        UPDATE_COLUMN.Add("OrderNo");
                    }
                    _OrderNo = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 審查標準範本 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_ReviewTemplate : IOFS_ReviewTemplate
    {
    }
    
}