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
    /// 社團類-基本資料 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_CLB_Application_Basic", "社團類-基本資料", false)]
    public class IOFS_CLB_Application_Basic : IMeta
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
        
        protected int? _Year = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Year", "Year", DataSource.TABLE, "", false)]
        public virtual int? Year
        {
            get
            {
                return _Year;
            }
            set
            {
                bool isModify = false;
                if (_Year == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Year.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Year") == -1)
                    {
                        UPDATE_COLUMN.Add("Year");
                    }
                    _Year = value;
                }
            }
        }
        
        protected int? _Serial = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Serial", "Serial", DataSource.TABLE, "", false)]
        public virtual int? Serial
        {
            get
            {
                return _Serial;
            }
            set
            {
                bool isModify = false;
                if (_Serial == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Serial.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Serial") == -1)
                    {
                        UPDATE_COLUMN.Add("Serial");
                    }
                    _Serial = value;
                }
            }
        }
        
        protected string _SubsidyPlanType = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SubsidyPlanType", "SubsidyPlanType", DataSource.TABLE, "", false)]
        public virtual string SubsidyPlanType
        {
            get
            {
                return _SubsidyPlanType;
            }
            set
            {
                bool isModify = false;
                if (_SubsidyPlanType == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SubsidyPlanType.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SubsidyPlanType") == -1)
                    {
                        UPDATE_COLUMN.Add("SubsidyPlanType");
                    }
                    _SubsidyPlanType = value;
                }
            }
        }
        
        protected string _ProjectNameTw = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProjectNameTw", "ProjectNameTw", DataSource.TABLE, "", false)]
        public virtual string ProjectNameTw
        {
            get
            {
                return _ProjectNameTw;
            }
            set
            {
                bool isModify = false;
                if (_ProjectNameTw == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ProjectNameTw.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ProjectNameTw") == -1)
                    {
                        UPDATE_COLUMN.Add("ProjectNameTw");
                    }
                    _ProjectNameTw = value;
                }
            }
        }
        
        protected string _SubsidyType = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SubsidyType", "SubsidyType", DataSource.TABLE, "", false)]
        public virtual string SubsidyType
        {
            get
            {
                return _SubsidyType;
            }
            set
            {
                bool isModify = false;
                if (_SubsidyType == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SubsidyType.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SubsidyType") == -1)
                    {
                        UPDATE_COLUMN.Add("SubsidyType");
                    }
                    _SubsidyType = value;
                }
            }
        }
        
        protected string _SchoolName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SchoolName", "SchoolName", DataSource.TABLE, "", false)]
        public virtual string SchoolName
        {
            get
            {
                return _SchoolName;
            }
            set
            {
                bool isModify = false;
                if (_SchoolName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SchoolName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SchoolName") == -1)
                    {
                        UPDATE_COLUMN.Add("SchoolName");
                    }
                    _SchoolName = value;
                }
            }
        }
        
        protected string _ClubName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ClubName", "ClubName", DataSource.TABLE, "", false)]
        public virtual string ClubName
        {
            get
            {
                return _ClubName;
            }
            set
            {
                bool isModify = false;
                if (_ClubName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ClubName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ClubName") == -1)
                    {
                        UPDATE_COLUMN.Add("ClubName");
                    }
                    _ClubName = value;
                }
            }
        }
        
        protected DateTime? _CreationDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreationDate", "CreationDate", DataSource.TABLE, "", false)]
        public virtual DateTime? CreationDate
        {
            get
            {
                return _CreationDate;
            }
            set
            {
                bool isModify = false;
                if (_CreationDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CreationDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreationDate") == -1)
                    {
                        UPDATE_COLUMN.Add("CreationDate");
                    }
                    _CreationDate = value;
                }
            }
        }
        
        protected string _School_IDNumber = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("School_IDNumber", "School_IDNumber", DataSource.TABLE, "", false)]
        public virtual string School_IDNumber
        {
            get
            {
                return _School_IDNumber;
            }
            set
            {
                bool isModify = false;
                if (_School_IDNumber == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_School_IDNumber.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("School_IDNumber") == -1)
                    {
                        UPDATE_COLUMN.Add("School_IDNumber");
                    }
                    _School_IDNumber = value;
                }
            }
        }
        
        protected string _Address = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Address", "Address", DataSource.TABLE, "", false)]
        public virtual string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                bool isModify = false;
                if (_Address == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Address.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Address") == -1)
                    {
                        UPDATE_COLUMN.Add("Address");
                    }
                    _Address = value;
                }
            }
        }
        
        protected DateTime? _created_at = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("created_at", "created_at", DataSource.TABLE, "", false)]
        public virtual DateTime? created_at
        {
            get
            {
                return _created_at;
            }
            set
            {
                bool isModify = false;
                if (_created_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_created_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("created_at") == -1)
                    {
                        UPDATE_COLUMN.Add("created_at");
                    }
                    _created_at = value;
                }
            }
        }
        
        protected DateTime? _updated_at = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("updated_at", "updated_at", DataSource.TABLE, "", false)]
        public virtual DateTime? updated_at
        {
            get
            {
                return _updated_at;
            }
            set
            {
                bool isModify = false;
                if (_updated_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_updated_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("updated_at") == -1)
                    {
                        UPDATE_COLUMN.Add("updated_at");
                    }
                    _updated_at = value;
                }
            }
        }
        
        protected int? _ApplyAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ApplyAmount", "ApplyAmount", DataSource.TABLE, "", false)]
        public virtual int? ApplyAmount
        {
            get
            {
                return _ApplyAmount;
            }
            set
            {
                bool isModify = false;
                if (_ApplyAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ApplyAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ApplyAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("ApplyAmount");
                    }
                    _ApplyAmount = value;
                }
            }
        }
        
        protected int? _SelfAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SelfAmount", "SelfAmount", DataSource.TABLE, "", false)]
        public virtual int? SelfAmount
        {
            get
            {
                return _SelfAmount;
            }
            set
            {
                bool isModify = false;
                if (_SelfAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SelfAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SelfAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("SelfAmount");
                    }
                    _SelfAmount = value;
                }
            }
        }
        
        protected int? _OtherAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OtherAmount", "OtherAmount", DataSource.TABLE, "", false)]
        public virtual int? OtherAmount
        {
            get
            {
                return _OtherAmount;
            }
            set
            {
                bool isModify = false;
                if (_OtherAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_OtherAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OtherAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("OtherAmount");
                    }
                    _OtherAmount = value;
                }
            }
        }
        
        protected bool? _IsPreviouslySubsidized = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsPreviouslySubsidized", "IsPreviouslySubsidized", DataSource.TABLE, "", false)]
        public virtual bool? IsPreviouslySubsidized
        {
            get
            {
                return _IsPreviouslySubsidized;
            }
            set
            {
                bool isModify = false;
                if (_IsPreviouslySubsidized == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsPreviouslySubsidized.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsPreviouslySubsidized") == -1)
                    {
                        UPDATE_COLUMN.Add("IsPreviouslySubsidized");
                    }
                    _IsPreviouslySubsidized = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 社團類-基本資料 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_CLB_Application_Basic : IOFS_CLB_Application_Basic
    {
    }
    
}