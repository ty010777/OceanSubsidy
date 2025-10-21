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
    /// 科專類-人事經費 (技術移轉、委託研究或驗證費)
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_PersonnelCost_ResearchFees", "科專類-人事經費", false)]
    public class IOFS_SCI_PersonnelCost_ResearchFees : IMeta
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
        
        protected string _FeeCategory = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FeeCategory", "FeeCategory", DataSource.TABLE, "", false)]
        public virtual string FeeCategory
        {
            get
            {
                return _FeeCategory;
            }
            set
            {
                bool isModify = false;
                if (_FeeCategory == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_FeeCategory.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FeeCategory") == -1)
                    {
                        UPDATE_COLUMN.Add("FeeCategory");
                    }
                    _FeeCategory = value;
                }
            }
        }
        
        protected DateTime? _StartDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartDate", "StartDate", DataSource.TABLE, "", false)]
        public virtual DateTime? StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                bool isModify = false;
                if (_StartDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StartDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartDate") == -1)
                    {
                        UPDATE_COLUMN.Add("StartDate");
                    }
                    _StartDate = value;
                }
            }
        }
        
        protected DateTime? _EndDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndDate", "EndDate", DataSource.TABLE, "", false)]
        public virtual DateTime? EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                bool isModify = false;
                if (_EndDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EndDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndDate") == -1)
                    {
                        UPDATE_COLUMN.Add("EndDate");
                    }
                    _EndDate = value;
                }
            }
        }
        
        protected string _Name = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Name", "Name", DataSource.TABLE, "", false)]
        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                bool isModify = false;
                if (_Name == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Name.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Name") == -1)
                    {
                        UPDATE_COLUMN.Add("Name");
                    }
                    _Name = value;
                }
            }
        }
        
        protected string _PersonName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PersonName", "PersonName", DataSource.TABLE, "", false)]
        public virtual string PersonName
        {
            get
            {
                return _PersonName;
            }
            set
            {
                bool isModify = false;
                if (_PersonName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PersonName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PersonName") == -1)
                    {
                        UPDATE_COLUMN.Add("PersonName");
                    }
                    _PersonName = value;
                }
            }
        }
        
        protected decimal? _Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Price", "Price", DataSource.TABLE, "", false)]
        public virtual decimal? Price
        {
            get
            {
                return _Price;
            }
            set
            {
                bool isModify = false;
                if (_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Price") == -1)
                    {
                        UPDATE_COLUMN.Add("Price");
                    }
                    _Price = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專類-人事經費 (技術移轉、委託研究或驗證費)
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_PersonnelCost_ResearchFees : IOFS_SCI_PersonnelCost_ResearchFees
    {
    }
    
}