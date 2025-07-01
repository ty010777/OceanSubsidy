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
    [GisTableAttribute("OFS_SCI_PersonnelCost_PersonForm", "", false)]
    public class IOFS_SCI_PersonnelCost_PersonForm : IMeta
    {
        
        protected string _Version_ID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Version_ID", "Version_ID", DataSource.TABLE, "", false)]
        public virtual string Version_ID
        {
            get
            {
                return _Version_ID;
            }
            set
            {
                bool isModify = false;
                if (_Version_ID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Version_ID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Version_ID") == -1)
                    {
                        UPDATE_COLUMN.Add("Version_ID");
                    }
                    _Version_ID = value;
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
        
        protected bool? _IsPending = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsPending", "IsPending", DataSource.TABLE, "", false)]
        public virtual bool? IsPending
        {
            get
            {
                return _IsPending;
            }
            set
            {
                bool isModify = false;
                if (_IsPending == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsPending.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsPending") == -1)
                    {
                        UPDATE_COLUMN.Add("IsPending");
                    }
                    _IsPending = value;
                }
            }
        }
        
        protected string _JobTitle = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("JobTitle", "JobTitle", DataSource.TABLE, "", false)]
        public virtual string JobTitle
        {
            get
            {
                return _JobTitle;
            }
            set
            {
                bool isModify = false;
                if (_JobTitle == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_JobTitle.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("JobTitle") == -1)
                    {
                        UPDATE_COLUMN.Add("JobTitle");
                    }
                    _JobTitle = value;
                }
            }
        }
        
        protected decimal? _AvgSalary = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AvgSalary", "AvgSalary", DataSource.TABLE, "", false)]
        public virtual decimal? AvgSalary
        {
            get
            {
                return _AvgSalary;
            }
            set
            {
                bool isModify = false;
                if (_AvgSalary == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_AvgSalary.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AvgSalary") == -1)
                    {
                        UPDATE_COLUMN.Add("AvgSalary");
                    }
                    _AvgSalary = value;
                }
            }
        }
        
        protected decimal? _Month = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Month", "Month", DataSource.TABLE, "", false)]
        public virtual decimal? Month
        {
            get
            {
                return _Month;
            }
            set
            {
                bool isModify = false;
                if (_Month == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Month.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Month") == -1)
                    {
                        UPDATE_COLUMN.Add("Month");
                    }
                    _Month = value;
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
    public partial class OFS_SCI_PersonnelCost_PersonForm : IOFS_SCI_PersonnelCost_PersonForm
    {
    }
    
}