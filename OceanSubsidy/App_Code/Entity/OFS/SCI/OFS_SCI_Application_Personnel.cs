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
    [GisTableAttribute("OFS_SCI_Application_Personnel", "", false)]
    public class IOFS_SCI_Application_Personnel : IMeta
    {
        
        protected int _idx = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("idx", "idx", DataSource.UN_OPERATE, "", true)]
        public virtual int idx
        {
            get
            {
                return _idx;
            }
            set
            {
                bool isModify = false;
                if (_idx == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_idx.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("idx") == -1)
                    {
                        UPDATE_COLUMN.Add("idx");
                    }
                    _idx = value;
                }
            }
        }
        
        protected string _PersonID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PersonID", "PersonID", DataSource.TABLE, "", false)]
        public virtual string PersonID
        {
            get
            {
                return _PersonID;
            }
            set
            {
                bool isModify = false;
                if (_PersonID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PersonID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PersonID") == -1)
                    {
                        UPDATE_COLUMN.Add("PersonID");
                    }
                    _PersonID = value;
                }
            }
        }
        
        protected string _Role = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Role", "Role", DataSource.TABLE, "", false)]
        public virtual string Role
        {
            get
            {
                return _Role;
            }
            set
            {
                bool isModify = false;
                if (_Role == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Role.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Role") == -1)
                    {
                        UPDATE_COLUMN.Add("Role");
                    }
                    _Role = value;
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
        
        protected string _Phone = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Phone", "Phone", DataSource.TABLE, "", false)]
        public virtual string Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                bool isModify = false;
                if (_Phone == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Phone.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Phone") == -1)
                    {
                        UPDATE_COLUMN.Add("Phone");
                    }
                    _Phone = value;
                }
            }
        }
        
        protected string _PhoneExt = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PhoneExt", "PhoneExt", DataSource.TABLE, "", false)]
        public virtual string PhoneExt
        {
            get
            {
                return _PhoneExt;
            }
            set
            {
                bool isModify = false;
                if (_PhoneExt == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PhoneExt.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PhoneExt") == -1)
                    {
                        UPDATE_COLUMN.Add("PhoneExt");
                    }
                    _PhoneExt = value;
                }
            }
        }
        
        protected string _MobilePhone = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("MobilePhone", "MobilePhone", DataSource.TABLE, "", false)]
        public virtual string MobilePhone
        {
            get
            {
                return _MobilePhone;
            }
            set
            {
                bool isModify = false;
                if (_MobilePhone == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_MobilePhone.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("MobilePhone") == -1)
                    {
                        UPDATE_COLUMN.Add("MobilePhone");
                    }
                    _MobilePhone = value;
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
    public partial class OFS_SCI_Application_Personnel : IOFS_SCI_Application_Personnel
    {
    }
    
}