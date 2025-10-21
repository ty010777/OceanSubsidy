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
    /// 科專類-人事經費 (消耗性器材及原材料費)
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_PersonnelCost_Material", "科專類-人事經費", false)]
    public class IOFS_SCI_PersonnelCost_Material : IMeta
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
        
        protected string _ItemName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ItemName", "ItemName", DataSource.TABLE, "", false)]
        public virtual string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                bool isModify = false;
                if (_ItemName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ItemName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ItemName") == -1)
                    {
                        UPDATE_COLUMN.Add("ItemName");
                    }
                    _ItemName = value;
                }
            }
        }
        
        protected string _Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Description", "Description", DataSource.TABLE, "", false)]
        public virtual string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                bool isModify = false;
                if (_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Description");
                    }
                    _Description = value;
                }
            }
        }
        
        protected string _Unit = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Unit", "Unit", DataSource.TABLE, "", false)]
        public virtual string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                bool isModify = false;
                if (_Unit == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Unit.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Unit") == -1)
                    {
                        UPDATE_COLUMN.Add("Unit");
                    }
                    _Unit = value;
                }
            }
        }
        
        protected decimal? _PreNum = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PreNum", "PreNum", DataSource.TABLE, "", false)]
        public virtual decimal? PreNum
        {
            get
            {
                return _PreNum;
            }
            set
            {
                bool isModify = false;
                if (_PreNum == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PreNum.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PreNum") == -1)
                    {
                        UPDATE_COLUMN.Add("PreNum");
                    }
                    _PreNum = value;
                }
            }
        }
        
        protected decimal? _UnitPrice = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UnitPrice", "UnitPrice", DataSource.TABLE, "", false)]
        public virtual decimal? UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                bool isModify = false;
                if (_UnitPrice == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_UnitPrice.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UnitPrice") == -1)
                    {
                        UPDATE_COLUMN.Add("UnitPrice");
                    }
                    _UnitPrice = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專類-人事經費 (消耗性器材及原材料費)
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_PersonnelCost_Material : IOFS_SCI_PersonnelCost_Material
    {
    }
    
}