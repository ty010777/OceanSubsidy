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
    [GisTableAttribute("OFS_SCI_PageModifyNote", "", false)]
    public class IOFS_SCI_PageModifyNote : IMeta
    {
        
        protected int _id = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("id", "id", DataSource.UN_OPERATE, "", true)]
        public virtual int id
        {
            get
            {
                return _id;
            }
            set
            {
                bool isModify = false;
                if (_id == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_id.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("id") == -1)
                    {
                        UPDATE_COLUMN.Add("id");
                    }
                    _id = value;
                }
            }
        }
        
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
        
        protected string _SourcePage = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SourcePage", "SourcePage", DataSource.TABLE, "", false)]
        public virtual string SourcePage
        {
            get
            {
                return _SourcePage;
            }
            set
            {
                bool isModify = false;
                if (_SourcePage == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SourcePage.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SourcePage") == -1)
                    {
                        UPDATE_COLUMN.Add("SourcePage");
                    }
                    _SourcePage = value;
                }
            }
        }
        
        protected string _ChangeBefore = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ChangeBefore", "ChangeBefore", DataSource.TABLE, "", false)]
        public virtual string ChangeBefore
        {
            get
            {
                return _ChangeBefore;
            }
            set
            {
                bool isModify = false;
                if (_ChangeBefore == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ChangeBefore.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ChangeBefore") == -1)
                    {
                        UPDATE_COLUMN.Add("ChangeBefore");
                    }
                    _ChangeBefore = value;
                }
            }
        }
        
        protected string _ChangeAfter = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ChangeAfter", "ChangeAfter", DataSource.TABLE, "", false)]
        public virtual string ChangeAfter
        {
            get
            {
                return _ChangeAfter;
            }
            set
            {
                bool isModify = false;
                if (_ChangeAfter == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ChangeAfter.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ChangeAfter") == -1)
                    {
                        UPDATE_COLUMN.Add("ChangeAfter");
                    }
                    _ChangeAfter = value;
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
    public partial class OFS_SCI_PageModifyNote : IOFS_SCI_PageModifyNote
    {
    }
    
}