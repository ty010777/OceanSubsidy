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
    [GisTableAttribute("Sys_ZgsCode", "", false)]
    public class ISys_ZgsCode : IMeta
    {
        
        protected string _CodeGroup = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CodeGroup", "CodeGroup", DataSource.TABLE, "", false)]
        public virtual string CodeGroup
        {
            get
            {
                return _CodeGroup;
            }
            set
            {
                bool isModify = false;
                if (_CodeGroup == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CodeGroup.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CodeGroup") == -1)
                    {
                        UPDATE_COLUMN.Add("CodeGroup");
                    }
                    _CodeGroup = value;
                }
            }
        }
        
        protected string _Code = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Code", "Code", DataSource.TABLE, "", false)]
        public virtual string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                bool isModify = false;
                if (_Code == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Code.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Code") == -1)
                    {
                        UPDATE_COLUMN.Add("Code");
                    }
                    _Code = value;
                }
            }
        }
        
        protected string _Descname = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Descname", "Descname", DataSource.TABLE, "", false)]
        public virtual string Descname
        {
            get
            {
                return _Descname;
            }
            set
            {
                bool isModify = false;
                if (_Descname == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Descname.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Descname") == -1)
                    {
                        UPDATE_COLUMN.Add("Descname");
                    }
                    _Descname = value;
                }
            }
        }
        
        protected int _OrderNo = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OrderNo", "OrderNo", DataSource.TABLE, "", false)]
        public virtual int OrderNo
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
        
        protected string _IsValid = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "", false)]
        public virtual string IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                bool isModify = false;
                if (_IsValid == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsValid.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsValid") == -1)
                    {
                        UPDATE_COLUMN.Add("IsValid");
                    }
                    _IsValid = value;
                }
            }
        }
        
        protected decimal? _MaxPriceLimit = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("MaxPriceLimit", "MaxPriceLimit", DataSource.TABLE, "", false)]
        public virtual decimal? MaxPriceLimit
        {
            get
            {
                return _MaxPriceLimit;
            }
            set
            {
                bool isModify = false;
                if (_MaxPriceLimit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_MaxPriceLimit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("MaxPriceLimit") == -1)
                    {
                        UPDATE_COLUMN.Add("MaxPriceLimit");
                    }
                    _MaxPriceLimit = value;
                }
            }
        }
        
        protected string _ValidBeginDate = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ValidBeginDate", "ValidBeginDate", DataSource.TABLE, "", false)]
        public virtual string ValidBeginDate
        {
            get
            {
                return _ValidBeginDate;
            }
            set
            {
                bool isModify = false;
                if (_ValidBeginDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ValidBeginDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ValidBeginDate") == -1)
                    {
                        UPDATE_COLUMN.Add("ValidBeginDate");
                    }
                    _ValidBeginDate = value;
                }
            }
        }
        
        protected string _ValidEndDate = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ValidEndDate", "ValidEndDate", DataSource.TABLE, "", false)]
        public virtual string ValidEndDate
        {
            get
            {
                return _ValidEndDate;
            }
            set
            {
                bool isModify = false;
                if (_ValidEndDate == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ValidEndDate.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ValidEndDate") == -1)
                    {
                        UPDATE_COLUMN.Add("ValidEndDate");
                    }
                    _ValidEndDate = value;
                }
            }
        }
        
        protected string _ParentCode = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ParentCode", "ParentCode", DataSource.TABLE, "", false)]
        public virtual string ParentCode
        {
            get
            {
                return _ParentCode;
            }
            set
            {
                bool isModify = false;
                if (_ParentCode == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ParentCode.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ParentCode") == -1)
                    {
                        UPDATE_COLUMN.Add("ParentCode");
                    }
                    _ParentCode = value;
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
    public partial class Sys_ZgsCode : ISys_ZgsCode
    {
    }
    
}