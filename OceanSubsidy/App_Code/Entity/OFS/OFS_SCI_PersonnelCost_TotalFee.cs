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
    [GisTableAttribute("OFS_SCI_PersonnelCost_TotalFee", "", false)]
    public class IOFS_SCI_PersonnelCost_TotalFee : IMeta
    {
        
        protected int _ID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ID", "ID", DataSource.UN_OPERATE, "", true)]
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
        
        protected string _Version_ID = null;
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
        
        protected string _AccountingItem = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AccountingItem", "AccountingItem", DataSource.TABLE, "", false)]
        public virtual string AccountingItem
        {
            get
            {
                return _AccountingItem;
            }
            set
            {
                bool isModify = false;
                if (_AccountingItem == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_AccountingItem.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AccountingItem") == -1)
                    {
                        UPDATE_COLUMN.Add("AccountingItem");
                    }
                    _AccountingItem = value;
                }
            }
        }
        
        protected decimal? _SubsidyAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SubsidyAmount", "SubsidyAmount", DataSource.TABLE, "", false)]
        public virtual decimal? SubsidyAmount
        {
            get
            {
                return _SubsidyAmount;
            }
            set
            {
                bool isModify = false;
                if (_SubsidyAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SubsidyAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SubsidyAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("SubsidyAmount");
                    }
                    _SubsidyAmount = value;
                }
            }
        }
        
        protected decimal? _CoopAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CoopAmount", "CoopAmount", DataSource.TABLE, "", false)]
        public virtual decimal? CoopAmount
        {
            get
            {
                return _CoopAmount;
            }
            set
            {
                bool isModify = false;
                if (_CoopAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CoopAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CoopAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("CoopAmount");
                    }
                    _CoopAmount = value;
                }
            }
        }
        
        protected decimal? _TotalAmount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TotalAmount", "TotalAmount", DataSource.UN_OPERATE, "", false)]
        public virtual decimal? TotalAmount
        {
            get
            {
                return _TotalAmount;
            }
            set
            {
                bool isModify = false;
                if (_TotalAmount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TotalAmount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TotalAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("TotalAmount");
                    }
                    _TotalAmount = value;
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
    public partial class OFS_SCI_PersonnelCost_TotalFee : IOFS_SCI_PersonnelCost_TotalFee
    {
    }
    
}