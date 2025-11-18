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
    [GisTableAttribute("OFS_CLB_Budget_Plan", "", false)]
    public class IOFS_CLB_Budget_Plan : IMeta
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
        
        protected string _Title = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Title", "Title", DataSource.TABLE, "", false)]
        public virtual string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                bool isModify = false;
                if (_Title == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Title.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Title") == -1)
                    {
                        UPDATE_COLUMN.Add("Title");
                    }
                    _Title = value;
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
    public partial class OFS_CLB_Budget_Plan : IOFS_CLB_Budget_Plan
    {
    }
    
}