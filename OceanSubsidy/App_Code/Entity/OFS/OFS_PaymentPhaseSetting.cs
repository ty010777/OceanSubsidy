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
    [GisTableAttribute("OFS_PaymentPhaseSetting", "", false)]
    public class IOFS_PaymentPhaseSetting : IMeta
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
        
        protected int? _PhaseOrder = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PhaseOrder", "PhaseOrder", DataSource.TABLE, "", false)]
        public virtual int? PhaseOrder
        {
            get
            {
                return _PhaseOrder;
            }
            set
            {
                bool isModify = false;
                if (_PhaseOrder == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PhaseOrder.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PhaseOrder") == -1)
                    {
                        UPDATE_COLUMN.Add("PhaseOrder");
                    }
                    _PhaseOrder = value;
                }
            }
        }
        
        protected string _PhaseName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PhaseName", "PhaseName", DataSource.TABLE, "", false)]
        public virtual string PhaseName
        {
            get
            {
                return _PhaseName;
            }
            set
            {
                bool isModify = false;
                if (_PhaseName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PhaseName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PhaseName") == -1)
                    {
                        UPDATE_COLUMN.Add("PhaseName");
                    }
                    _PhaseName = value;
                }
            }
        }
        
        protected decimal? _DisbursementRatioPct = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DisbursementRatioPct", "DisbursementRatioPct", DataSource.TABLE, "", false)]
        public virtual decimal? DisbursementRatioPct
        {
            get
            {
                return _DisbursementRatioPct;
            }
            set
            {
                bool isModify = false;
                if (_DisbursementRatioPct == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_DisbursementRatioPct.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DisbursementRatioPct") == -1)
                    {
                        UPDATE_COLUMN.Add("DisbursementRatioPct");
                    }
                    _DisbursementRatioPct = value;
                }
            }
        }
        
        protected string _DisbursementType = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DisbursementType", "DisbursementType", DataSource.TABLE, "", false)]
        public virtual string DisbursementType
        {
            get
            {
                return _DisbursementType;
            }
            set
            {
                bool isModify = false;
                if (_DisbursementType == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_DisbursementType.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DisbursementType") == -1)
                    {
                        UPDATE_COLUMN.Add("DisbursementType");
                    }
                    _DisbursementType = value;
                }
            }
        }
        
        protected string _Note = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Note", "Note", DataSource.TABLE, "", false)]
        public virtual string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                bool isModify = false;
                if (_Note == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Note.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Note") == -1)
                    {
                        UPDATE_COLUMN.Add("Note");
                    }
                    _Note = value;
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
    public partial class OFS_PaymentPhaseSetting : IOFS_PaymentPhaseSetting
    {
    }
    
}