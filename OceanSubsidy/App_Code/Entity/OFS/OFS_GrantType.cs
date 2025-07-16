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
    /// OFS補助案資助類型 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_GrantType", "OFS補助案資助類型", false)]
    public class IOFS_GrantType : IMeta
    {
        
        protected int _TypeID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TypeID", "TypeID", DataSource.UN_OPERATE, "", false)]
        public virtual int TypeID
        {
            get
            {
                return _TypeID;
            }
            set
            {
                bool isModify = false;
                if (_TypeID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TypeID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TypeID") == -1)
                    {
                        UPDATE_COLUMN.Add("TypeID");
                    }
                    _TypeID = value;
                }
            }
        }
        
        protected string _TypeCode = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TypeCode", "TypeCode", DataSource.TABLE, "", false)]
        public virtual string TypeCode
        {
            get
            {
                return _TypeCode;
            }
            set
            {
                bool isModify = false;
                if (_TypeCode == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TypeCode.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TypeCode") == -1)
                    {
                        UPDATE_COLUMN.Add("TypeCode");
                    }
                    _TypeCode = value;
                }
            }
        }
        
        protected string _ShortName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ShortName", "ShortName", DataSource.TABLE, "", false)]
        public virtual string ShortName
        {
            get
            {
                return _ShortName;
            }
            set
            {
                bool isModify = false;
                if (_ShortName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ShortName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ShortName") == -1)
                    {
                        UPDATE_COLUMN.Add("ShortName");
                    }
                    _ShortName = value;
                }
            }
        }
        
        protected string _FullName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FullName", "FullName", DataSource.TABLE, "", false)]
        public virtual string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                bool isModify = false;
                if (_FullName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_FullName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FullName") == -1)
                    {
                        UPDATE_COLUMN.Add("FullName");
                    }
                    _FullName = value;
                }
            }
        }
        
        protected DateTime? _ApplyStartDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ApplyStartDate", "ApplyStartDate", DataSource.TABLE, "", false)]
        public virtual DateTime? ApplyStartDate
        {
            get
            {
                return _ApplyStartDate;
            }
            set
            {
                bool isModify = false;
                if (_ApplyStartDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ApplyStartDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ApplyStartDate") == -1)
                    {
                        UPDATE_COLUMN.Add("ApplyStartDate");
                    }
                    _ApplyStartDate = value;
                }
            }
        }
        
        protected DateTime? _ApplyEndDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ApplyEndDate", "ApplyEndDate", DataSource.TABLE, "", false)]
        public virtual DateTime? ApplyEndDate
        {
            get
            {
                return _ApplyEndDate;
            }
            set
            {
                bool isModify = false;
                if (_ApplyEndDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ApplyEndDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ApplyEndDate") == -1)
                    {
                        UPDATE_COLUMN.Add("ApplyEndDate");
                    }
                    _ApplyEndDate = value;
                }
            }
        }
        
        protected string _AdminUnit = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AdminUnit", "AdminUnit", DataSource.TABLE, "", false)]
        public virtual string AdminUnit
        {
            get
            {
                return _AdminUnit;
            }
            set
            {
                bool isModify = false;
                if (_AdminUnit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_AdminUnit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AdminUnit") == -1)
                    {
                        UPDATE_COLUMN.Add("AdminUnit");
                    }
                    _AdminUnit = value;
                }
            }
        }
        
        protected int? _PlanEndDate = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PlanEndDate", "PlanEndDate", DataSource.TABLE, "", false)]
        public virtual int? PlanEndDate
        {
            get
            {
                return _PlanEndDate;
            }
            set
            {
                bool isModify = false;
                if (_PlanEndDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PlanEndDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PlanEndDate") == -1)
                    {
                        UPDATE_COLUMN.Add("PlanEndDate");
                    }
                    _PlanEndDate = value;
                }
            }
        }
        
        protected decimal? _BudgetFees = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("BudgetFees", "BudgetFees", DataSource.TABLE, "", false)]
        public virtual decimal? BudgetFees
        {
            get
            {
                return _BudgetFees;
            }
            set
            {
                bool isModify = false;
                if (_BudgetFees == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_BudgetFees.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("BudgetFees") == -1)
                    {
                        UPDATE_COLUMN.Add("BudgetFees");
                    }
                    _BudgetFees = value;
                }
            }
        }
        
        protected int? _OverduePeriod = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OverduePeriod", "OverduePeriod", DataSource.TABLE, "", false)]
        public virtual int? OverduePeriod
        {
            get
            {
                return _OverduePeriod;
            }
            set
            {
                bool isModify = false;
                if (_OverduePeriod == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_OverduePeriod.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OverduePeriod") == -1)
                    {
                        UPDATE_COLUMN.Add("OverduePeriod");
                    }
                    _OverduePeriod = value;
                }
            }
        }
        
        protected string _TargetTags = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TargetTags", "TargetTags", DataSource.TABLE, "", false)]
        public virtual string TargetTags
        {
            get
            {
                return _TargetTags;
            }
            set
            {
                bool isModify = false;
                if (_TargetTags == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TargetTags.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TargetTags") == -1)
                    {
                        UPDATE_COLUMN.Add("TargetTags");
                    }
                    _TargetTags = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// OFS補助案資助類型 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_GrantType : IOFS_GrantType
    {
    }
    
}