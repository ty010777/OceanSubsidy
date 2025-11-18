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
    [GisTableAttribute("OFS_CLB_Other_Subsidy", "", false)]
    public class IOFS_CLB_Other_Subsidy : IMeta
    {
        
        protected int _ID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ID", "ID", DataSource.TABLE, "", false)]
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
        
        protected string _ProjectID = null;
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
        
        protected int? _Amount = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Amount", "Amount", DataSource.TABLE, "", false)]
        public virtual int? Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                bool isModify = false;
                if (_Amount == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Amount.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Amount") == -1)
                    {
                        UPDATE_COLUMN.Add("Amount");
                    }
                    _Amount = value;
                }
            }
        }
        
        protected string _Content = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Content", "Content", DataSource.TABLE, "", false)]
        public virtual string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                bool isModify = false;
                if (_Content == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Content.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Content") == -1)
                    {
                        UPDATE_COLUMN.Add("Content");
                    }
                    _Content = value;
                }
            }
        }
        
        protected DateTime _CreateTime = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreateTime", "CreateTime", DataSource.TABLE, "", false)]
        public virtual DateTime CreateTime
        {
            get
            {
                return _CreateTime;
            }
            set
            {
                bool isModify = false;
                if (_CreateTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CreateTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreateTime") == -1)
                    {
                        UPDATE_COLUMN.Add("CreateTime");
                    }
                    _CreateTime = value;
                }
            }
        }
        
        protected DateTime? _UpdateTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UpdateTime", "UpdateTime", DataSource.TABLE, "", false)]
        public virtual DateTime? UpdateTime
        {
            get
            {
                return _UpdateTime;
            }
            set
            {
                bool isModify = false;
                if (_UpdateTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_UpdateTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UpdateTime") == -1)
                    {
                        UPDATE_COLUMN.Add("UpdateTime");
                    }
                    _UpdateTime = value;
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
    public partial class OFS_CLB_Other_Subsidy : IOFS_CLB_Other_Subsidy
    {
    }
    
}