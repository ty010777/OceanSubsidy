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
    [GisTableAttribute("OFS_CLB_Application_Personnel", "", false)]
    public class IOFS_CLB_Application_Personnel : IMeta
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
        
        protected string _Personnel = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Personnel", "Personnel", DataSource.TABLE, "", false)]
        public virtual string Personnel
        {
            get
            {
                return _Personnel;
            }
            set
            {
                bool isModify = false;
                if (_Personnel == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Personnel.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Personnel") == -1)
                    {
                        UPDATE_COLUMN.Add("Personnel");
                    }
                    _Personnel = value;
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
        
        protected string _PhoneNum = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PhoneNum", "PhoneNum", DataSource.TABLE, "", false)]
        public virtual string PhoneNum
        {
            get
            {
                return _PhoneNum;
            }
            set
            {
                bool isModify = false;
                if (_PhoneNum == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PhoneNum.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PhoneNum") == -1)
                    {
                        UPDATE_COLUMN.Add("PhoneNum");
                    }
                    _PhoneNum = value;
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
    public partial class OFS_CLB_Application_Personnel : IOFS_CLB_Application_Personnel
    {
    }
    
}