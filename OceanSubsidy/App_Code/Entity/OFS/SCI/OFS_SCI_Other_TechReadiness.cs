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
    [GisTableAttribute("OFS_SCI_Other_TechReadiness", "", false)]
    public class IOFS_SCI_Other_TechReadiness : IMeta
    {
        
        protected string _Version_ID = "";
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
        
        protected string _Bef_TRLevel = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Bef_TRLevel", "Bef_TRLevel", DataSource.TABLE, "", false)]
        public virtual string Bef_TRLevel
        {
            get
            {
                return _Bef_TRLevel;
            }
            set
            {
                bool isModify = false;
                if (_Bef_TRLevel == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Bef_TRLevel.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Bef_TRLevel") == -1)
                    {
                        UPDATE_COLUMN.Add("Bef_TRLevel");
                    }
                    _Bef_TRLevel = value;
                }
            }
        }
        
        protected string _Aft_TRLevel = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Aft_TRLevel", "Aft_TRLevel", DataSource.TABLE, "", false)]
        public virtual string Aft_TRLevel
        {
            get
            {
                return _Aft_TRLevel;
            }
            set
            {
                bool isModify = false;
                if (_Aft_TRLevel == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Aft_TRLevel.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Aft_TRLevel") == -1)
                    {
                        UPDATE_COLUMN.Add("Aft_TRLevel");
                    }
                    _Aft_TRLevel = value;
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
    public partial class OFS_SCI_Other_TechReadiness : IOFS_SCI_Other_TechReadiness
    {
    }
    
}