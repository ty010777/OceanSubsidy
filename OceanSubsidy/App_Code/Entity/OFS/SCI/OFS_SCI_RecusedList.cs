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
    [GisTableAttribute("OFS_SCI_Other_Recused", "", false)]
    public class IOFS_SCI_Other_Recused : IMeta
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
        
        protected string _RecusedName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RecusedName", "RecusedName", DataSource.TABLE, "", false)]
        public virtual string RecusedName
        {
            get
            {
                return _RecusedName;
            }
            set
            {
                bool isModify = false;
                if (_RecusedName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_RecusedName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("RecusedName") == -1)
                    {
                        UPDATE_COLUMN.Add("RecusedName");
                    }
                    _RecusedName = value;
                }
            }
        }
        
        protected string _EmploymentUnit = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EmploymentUnit", "EmploymentUnit", DataSource.TABLE, "", false)]
        public virtual string EmploymentUnit
        {
            get
            {
                return _EmploymentUnit;
            }
            set
            {
                bool isModify = false;
                if (_EmploymentUnit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EmploymentUnit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EmploymentUnit") == -1)
                    {
                        UPDATE_COLUMN.Add("EmploymentUnit");
                    }
                    _EmploymentUnit = value;
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
        
        protected string _RecusedReason = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RecusedReason", "RecusedReason", DataSource.TABLE, "", false)]
        public virtual string RecusedReason
        {
            get
            {
                return _RecusedReason;
            }
            set
            {
                bool isModify = false;
                if (_RecusedReason == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_RecusedReason.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("RecusedReason") == -1)
                    {
                        UPDATE_COLUMN.Add("RecusedReason");
                    }
                    _RecusedReason = value;
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
    public partial class OFS_SCI_Other_Recused : IOFS_SCI_Other_Recused
    {
    }
    
}