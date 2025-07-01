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
    [GisTableAttribute("OFS_SCI_Outcomes", "", false)]
    public class IOFS_SCI_Outcomes : IMeta
    {
        
        protected string _Version_ID = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Version_ID", "Version_ID", DataSource.TABLE, "", true)]
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
        
        protected int? _TechTransfer_Plan_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechTransfer_Plan_Count", "TechTransfer_Plan_Count", DataSource.TABLE, "", false)]
        public virtual int? TechTransfer_Plan_Count
        {
            get
            {
                return _TechTransfer_Plan_Count;
            }
            set
            {
                bool isModify = false;
                if (_TechTransfer_Plan_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechTransfer_Plan_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechTransfer_Plan_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("TechTransfer_Plan_Count");
                    }
                    _TechTransfer_Plan_Count = value;
                }
            }
        }
        
        protected decimal? _TechTransfer_Plan_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechTransfer_Plan_Price", "TechTransfer_Plan_Price", DataSource.TABLE, "", false)]
        public virtual decimal? TechTransfer_Plan_Price
        {
            get
            {
                return _TechTransfer_Plan_Price;
            }
            set
            {
                bool isModify = false;
                if (_TechTransfer_Plan_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechTransfer_Plan_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechTransfer_Plan_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("TechTransfer_Plan_Price");
                    }
                    _TechTransfer_Plan_Price = value;
                }
            }
        }
        
        protected int? _TechTransfer_Track_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechTransfer_Track_Count", "TechTransfer_Track_Count", DataSource.TABLE, "", false)]
        public virtual int? TechTransfer_Track_Count
        {
            get
            {
                return _TechTransfer_Track_Count;
            }
            set
            {
                bool isModify = false;
                if (_TechTransfer_Track_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechTransfer_Track_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechTransfer_Track_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("TechTransfer_Track_Count");
                    }
                    _TechTransfer_Track_Count = value;
                }
            }
        }
        
        protected decimal? _TechTransfer_Track_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechTransfer_Track_Price", "TechTransfer_Track_Price", DataSource.TABLE, "", false)]
        public virtual decimal? TechTransfer_Track_Price
        {
            get
            {
                return _TechTransfer_Track_Price;
            }
            set
            {
                bool isModify = false;
                if (_TechTransfer_Track_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechTransfer_Track_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechTransfer_Track_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("TechTransfer_Track_Price");
                    }
                    _TechTransfer_Track_Price = value;
                }
            }
        }
        
        protected string _TechTransfer_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechTransfer_Description", "TechTransfer_Description", DataSource.TABLE, "", false)]
        public virtual string TechTransfer_Description
        {
            get
            {
                return _TechTransfer_Description;
            }
            set
            {
                bool isModify = false;
                if (_TechTransfer_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechTransfer_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechTransfer_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("TechTransfer_Description");
                    }
                    _TechTransfer_Description = value;
                }
            }
        }
        
        protected int? _Patent_Plan_Apply = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Patent_Plan_Apply", "Patent_Plan_Apply", DataSource.TABLE, "", false)]
        public virtual int? Patent_Plan_Apply
        {
            get
            {
                return _Patent_Plan_Apply;
            }
            set
            {
                bool isModify = false;
                if (_Patent_Plan_Apply == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Patent_Plan_Apply.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Patent_Plan_Apply") == -1)
                    {
                        UPDATE_COLUMN.Add("Patent_Plan_Apply");
                    }
                    _Patent_Plan_Apply = value;
                }
            }
        }
        
        protected int? _Patent_Plan_Grant = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Patent_Plan_Grant", "Patent_Plan_Grant", DataSource.TABLE, "", false)]
        public virtual int? Patent_Plan_Grant
        {
            get
            {
                return _Patent_Plan_Grant;
            }
            set
            {
                bool isModify = false;
                if (_Patent_Plan_Grant == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Patent_Plan_Grant.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Patent_Plan_Grant") == -1)
                    {
                        UPDATE_COLUMN.Add("Patent_Plan_Grant");
                    }
                    _Patent_Plan_Grant = value;
                }
            }
        }
        
        protected int? _Patent_Track_Apply = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Patent_Track_Apply", "Patent_Track_Apply", DataSource.TABLE, "", false)]
        public virtual int? Patent_Track_Apply
        {
            get
            {
                return _Patent_Track_Apply;
            }
            set
            {
                bool isModify = false;
                if (_Patent_Track_Apply == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Patent_Track_Apply.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Patent_Track_Apply") == -1)
                    {
                        UPDATE_COLUMN.Add("Patent_Track_Apply");
                    }
                    _Patent_Track_Apply = value;
                }
            }
        }
        
        protected int? _Patent_Track_Grant = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Patent_Track_Grant", "Patent_Track_Grant", DataSource.TABLE, "", false)]
        public virtual int? Patent_Track_Grant
        {
            get
            {
                return _Patent_Track_Grant;
            }
            set
            {
                bool isModify = false;
                if (_Patent_Track_Grant == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Patent_Track_Grant.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Patent_Track_Grant") == -1)
                    {
                        UPDATE_COLUMN.Add("Patent_Track_Grant");
                    }
                    _Patent_Track_Grant = value;
                }
            }
        }
        
        protected string _Patent_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Patent_Description", "Patent_Description", DataSource.TABLE, "", false)]
        public virtual string Patent_Description
        {
            get
            {
                return _Patent_Description;
            }
            set
            {
                bool isModify = false;
                if (_Patent_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Patent_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Patent_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Patent_Description");
                    }
                    _Patent_Description = value;
                }
            }
        }
        
        protected int? _Talent_Plan_PhD = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Talent_Plan_PhD", "Talent_Plan_PhD", DataSource.TABLE, "", false)]
        public virtual int? Talent_Plan_PhD
        {
            get
            {
                return _Talent_Plan_PhD;
            }
            set
            {
                bool isModify = false;
                if (_Talent_Plan_PhD == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Talent_Plan_PhD.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Talent_Plan_PhD") == -1)
                    {
                        UPDATE_COLUMN.Add("Talent_Plan_PhD");
                    }
                    _Talent_Plan_PhD = value;
                }
            }
        }
        
        protected int? _Talent_Plan_Master = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Talent_Plan_Master", "Talent_Plan_Master", DataSource.TABLE, "", false)]
        public virtual int? Talent_Plan_Master
        {
            get
            {
                return _Talent_Plan_Master;
            }
            set
            {
                bool isModify = false;
                if (_Talent_Plan_Master == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Talent_Plan_Master.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Talent_Plan_Master") == -1)
                    {
                        UPDATE_COLUMN.Add("Talent_Plan_Master");
                    }
                    _Talent_Plan_Master = value;
                }
            }
        }
        
        protected int? _Talent_Plan_Others = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Talent_Plan_Others", "Talent_Plan_Others", DataSource.TABLE, "", false)]
        public virtual int? Talent_Plan_Others
        {
            get
            {
                return _Talent_Plan_Others;
            }
            set
            {
                bool isModify = false;
                if (_Talent_Plan_Others == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Talent_Plan_Others.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Talent_Plan_Others") == -1)
                    {
                        UPDATE_COLUMN.Add("Talent_Plan_Others");
                    }
                    _Talent_Plan_Others = value;
                }
            }
        }
        
        protected int? _Talent_Track_PhD = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Talent_Track_PhD", "Talent_Track_PhD", DataSource.TABLE, "", false)]
        public virtual int? Talent_Track_PhD
        {
            get
            {
                return _Talent_Track_PhD;
            }
            set
            {
                bool isModify = false;
                if (_Talent_Track_PhD == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Talent_Track_PhD.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Talent_Track_PhD") == -1)
                    {
                        UPDATE_COLUMN.Add("Talent_Track_PhD");
                    }
                    _Talent_Track_PhD = value;
                }
            }
        }
        
        protected int? _Talent_Track_Master = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Talent_Track_Master", "Talent_Track_Master", DataSource.TABLE, "", false)]
        public virtual int? Talent_Track_Master
        {
            get
            {
                return _Talent_Track_Master;
            }
            set
            {
                bool isModify = false;
                if (_Talent_Track_Master == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Talent_Track_Master.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Talent_Track_Master") == -1)
                    {
                        UPDATE_COLUMN.Add("Talent_Track_Master");
                    }
                    _Talent_Track_Master = value;
                }
            }
        }
        
        protected int? _Talent_Track_Others = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Talent_Track_Others", "Talent_Track_Others", DataSource.TABLE, "", false)]
        public virtual int? Talent_Track_Others
        {
            get
            {
                return _Talent_Track_Others;
            }
            set
            {
                bool isModify = false;
                if (_Talent_Track_Others == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Talent_Track_Others.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Talent_Track_Others") == -1)
                    {
                        UPDATE_COLUMN.Add("Talent_Track_Others");
                    }
                    _Talent_Track_Others = value;
                }
            }
        }
        
        protected string _Talent_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Talent_Description", "Talent_Description", DataSource.TABLE, "", false)]
        public virtual string Talent_Description
        {
            get
            {
                return _Talent_Description;
            }
            set
            {
                bool isModify = false;
                if (_Talent_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Talent_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Talent_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Talent_Description");
                    }
                    _Talent_Description = value;
                }
            }
        }
        
        protected int? _Papers_Plan = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Papers_Plan", "Papers_Plan", DataSource.TABLE, "", false)]
        public virtual int? Papers_Plan
        {
            get
            {
                return _Papers_Plan;
            }
            set
            {
                bool isModify = false;
                if (_Papers_Plan == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Papers_Plan.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Papers_Plan") == -1)
                    {
                        UPDATE_COLUMN.Add("Papers_Plan");
                    }
                    _Papers_Plan = value;
                }
            }
        }
        
        protected int? _Papers_Track = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Papers_Track", "Papers_Track", DataSource.TABLE, "", false)]
        public virtual int? Papers_Track
        {
            get
            {
                return _Papers_Track;
            }
            set
            {
                bool isModify = false;
                if (_Papers_Track == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Papers_Track.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Papers_Track") == -1)
                    {
                        UPDATE_COLUMN.Add("Papers_Track");
                    }
                    _Papers_Track = value;
                }
            }
        }
        
        protected string _Papers_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Papers_Description", "Papers_Description", DataSource.TABLE, "", false)]
        public virtual string Papers_Description
        {
            get
            {
                return _Papers_Description;
            }
            set
            {
                bool isModify = false;
                if (_Papers_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Papers_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Papers_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Papers_Description");
                    }
                    _Papers_Description = value;
                }
            }
        }
        
        protected int? _IndustryCollab_Plan_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IndustryCollab_Plan_Count", "IndustryCollab_Plan_Count", DataSource.TABLE, "", false)]
        public virtual int? IndustryCollab_Plan_Count
        {
            get
            {
                return _IndustryCollab_Plan_Count;
            }
            set
            {
                bool isModify = false;
                if (_IndustryCollab_Plan_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IndustryCollab_Plan_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IndustryCollab_Plan_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("IndustryCollab_Plan_Count");
                    }
                    _IndustryCollab_Plan_Count = value;
                }
            }
        }
        
        protected decimal? _IndustryCollab_Plan_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IndustryCollab_Plan_Price", "IndustryCollab_Plan_Price", DataSource.TABLE, "", false)]
        public virtual decimal? IndustryCollab_Plan_Price
        {
            get
            {
                return _IndustryCollab_Plan_Price;
            }
            set
            {
                bool isModify = false;
                if (_IndustryCollab_Plan_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IndustryCollab_Plan_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IndustryCollab_Plan_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("IndustryCollab_Plan_Price");
                    }
                    _IndustryCollab_Plan_Price = value;
                }
            }
        }
        
        protected int? _IndustryCollab_Track_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IndustryCollab_Track_Count", "IndustryCollab_Track_Count", DataSource.TABLE, "", false)]
        public virtual int? IndustryCollab_Track_Count
        {
            get
            {
                return _IndustryCollab_Track_Count;
            }
            set
            {
                bool isModify = false;
                if (_IndustryCollab_Track_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IndustryCollab_Track_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IndustryCollab_Track_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("IndustryCollab_Track_Count");
                    }
                    _IndustryCollab_Track_Count = value;
                }
            }
        }
        
        protected decimal? _IndustryCollab_Track_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IndustryCollab_Track_Price", "IndustryCollab_Track_Price", DataSource.TABLE, "", false)]
        public virtual decimal? IndustryCollab_Track_Price
        {
            get
            {
                return _IndustryCollab_Track_Price;
            }
            set
            {
                bool isModify = false;
                if (_IndustryCollab_Track_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IndustryCollab_Track_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IndustryCollab_Track_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("IndustryCollab_Track_Price");
                    }
                    _IndustryCollab_Track_Price = value;
                }
            }
        }
        
        protected string _IndustryCollab_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IndustryCollab_Description", "IndustryCollab_Description", DataSource.TABLE, "", false)]
        public virtual string IndustryCollab_Description
        {
            get
            {
                return _IndustryCollab_Description;
            }
            set
            {
                bool isModify = false;
                if (_IndustryCollab_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IndustryCollab_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IndustryCollab_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("IndustryCollab_Description");
                    }
                    _IndustryCollab_Description = value;
                }
            }
        }
        
        protected decimal? _Investment_Plan_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Investment_Plan_Price", "Investment_Plan_Price", DataSource.TABLE, "", false)]
        public virtual decimal? Investment_Plan_Price
        {
            get
            {
                return _Investment_Plan_Price;
            }
            set
            {
                bool isModify = false;
                if (_Investment_Plan_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Investment_Plan_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Investment_Plan_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("Investment_Plan_Price");
                    }
                    _Investment_Plan_Price = value;
                }
            }
        }
        
        protected decimal? _Investment_Track_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Investment_Track_Price", "Investment_Track_Price", DataSource.TABLE, "", false)]
        public virtual decimal? Investment_Track_Price
        {
            get
            {
                return _Investment_Track_Price;
            }
            set
            {
                bool isModify = false;
                if (_Investment_Track_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Investment_Track_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Investment_Track_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("Investment_Track_Price");
                    }
                    _Investment_Track_Price = value;
                }
            }
        }
        
        protected string _Investment_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Investment_Description", "Investment_Description", DataSource.TABLE, "", false)]
        public virtual string Investment_Description
        {
            get
            {
                return _Investment_Description;
            }
            set
            {
                bool isModify = false;
                if (_Investment_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Investment_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Investment_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Investment_Description");
                    }
                    _Investment_Description = value;
                }
            }
        }
        
        protected int? _Products_Plan_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Products_Plan_Count", "Products_Plan_Count", DataSource.TABLE, "", false)]
        public virtual int? Products_Plan_Count
        {
            get
            {
                return _Products_Plan_Count;
            }
            set
            {
                bool isModify = false;
                if (_Products_Plan_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Products_Plan_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Products_Plan_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("Products_Plan_Count");
                    }
                    _Products_Plan_Count = value;
                }
            }
        }
        
        protected decimal? _Products_Plan_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Products_Plan_Price", "Products_Plan_Price", DataSource.TABLE, "", false)]
        public virtual decimal? Products_Plan_Price
        {
            get
            {
                return _Products_Plan_Price;
            }
            set
            {
                bool isModify = false;
                if (_Products_Plan_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Products_Plan_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Products_Plan_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("Products_Plan_Price");
                    }
                    _Products_Plan_Price = value;
                }
            }
        }
        
        protected int? _Products_Track_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Products_Track_Count", "Products_Track_Count", DataSource.TABLE, "", false)]
        public virtual int? Products_Track_Count
        {
            get
            {
                return _Products_Track_Count;
            }
            set
            {
                bool isModify = false;
                if (_Products_Track_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Products_Track_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Products_Track_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("Products_Track_Count");
                    }
                    _Products_Track_Count = value;
                }
            }
        }
        
        protected decimal? _Products_Track_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Products_Track_Price", "Products_Track_Price", DataSource.TABLE, "", false)]
        public virtual decimal? Products_Track_Price
        {
            get
            {
                return _Products_Track_Price;
            }
            set
            {
                bool isModify = false;
                if (_Products_Track_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Products_Track_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Products_Track_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("Products_Track_Price");
                    }
                    _Products_Track_Price = value;
                }
            }
        }
        
        protected string _Products_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Products_Description", "Products_Description", DataSource.TABLE, "", false)]
        public virtual string Products_Description
        {
            get
            {
                return _Products_Description;
            }
            set
            {
                bool isModify = false;
                if (_Products_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Products_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Products_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Products_Description");
                    }
                    _Products_Description = value;
                }
            }
        }
        
        protected decimal? _CostReduction_Plan_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CostReduction_Plan_Price", "CostReduction_Plan_Price", DataSource.TABLE, "", false)]
        public virtual decimal? CostReduction_Plan_Price
        {
            get
            {
                return _CostReduction_Plan_Price;
            }
            set
            {
                bool isModify = false;
                if (_CostReduction_Plan_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CostReduction_Plan_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CostReduction_Plan_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("CostReduction_Plan_Price");
                    }
                    _CostReduction_Plan_Price = value;
                }
            }
        }
        
        protected decimal? _CostReduction_Track_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CostReduction_Track_Price", "CostReduction_Track_Price", DataSource.TABLE, "", false)]
        public virtual decimal? CostReduction_Track_Price
        {
            get
            {
                return _CostReduction_Track_Price;
            }
            set
            {
                bool isModify = false;
                if (_CostReduction_Track_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CostReduction_Track_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CostReduction_Track_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("CostReduction_Track_Price");
                    }
                    _CostReduction_Track_Price = value;
                }
            }
        }
        
        protected string _CostReduction_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CostReduction_Description", "CostReduction_Description", DataSource.TABLE, "", false)]
        public virtual string CostReduction_Description
        {
            get
            {
                return _CostReduction_Description;
            }
            set
            {
                bool isModify = false;
                if (_CostReduction_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CostReduction_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CostReduction_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("CostReduction_Description");
                    }
                    _CostReduction_Description = value;
                }
            }
        }
        
        protected int? _PromoEvents_Plan = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PromoEvents_Plan", "PromoEvents_Plan", DataSource.TABLE, "", false)]
        public virtual int? PromoEvents_Plan
        {
            get
            {
                return _PromoEvents_Plan;
            }
            set
            {
                bool isModify = false;
                if (_PromoEvents_Plan == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PromoEvents_Plan.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PromoEvents_Plan") == -1)
                    {
                        UPDATE_COLUMN.Add("PromoEvents_Plan");
                    }
                    _PromoEvents_Plan = value;
                }
            }
        }
        
        protected int? _PromoEvents_Track = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PromoEvents_Track", "PromoEvents_Track", DataSource.TABLE, "", false)]
        public virtual int? PromoEvents_Track
        {
            get
            {
                return _PromoEvents_Track;
            }
            set
            {
                bool isModify = false;
                if (_PromoEvents_Track == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PromoEvents_Track.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PromoEvents_Track") == -1)
                    {
                        UPDATE_COLUMN.Add("PromoEvents_Track");
                    }
                    _PromoEvents_Track = value;
                }
            }
        }
        
        protected string _PromoEvents_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PromoEvents_Description", "PromoEvents_Description", DataSource.TABLE, "", false)]
        public virtual string PromoEvents_Description
        {
            get
            {
                return _PromoEvents_Description;
            }
            set
            {
                bool isModify = false;
                if (_PromoEvents_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PromoEvents_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PromoEvents_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("PromoEvents_Description");
                    }
                    _PromoEvents_Description = value;
                }
            }
        }
        
        protected int? _TechServices_Plan_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechServices_Plan_Count", "TechServices_Plan_Count", DataSource.TABLE, "", false)]
        public virtual int? TechServices_Plan_Count
        {
            get
            {
                return _TechServices_Plan_Count;
            }
            set
            {
                bool isModify = false;
                if (_TechServices_Plan_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechServices_Plan_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechServices_Plan_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("TechServices_Plan_Count");
                    }
                    _TechServices_Plan_Count = value;
                }
            }
        }
        
        protected decimal? _TechServices_Plan_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechServices_Plan_Price", "TechServices_Plan_Price", DataSource.TABLE, "", false)]
        public virtual decimal? TechServices_Plan_Price
        {
            get
            {
                return _TechServices_Plan_Price;
            }
            set
            {
                bool isModify = false;
                if (_TechServices_Plan_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechServices_Plan_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechServices_Plan_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("TechServices_Plan_Price");
                    }
                    _TechServices_Plan_Price = value;
                }
            }
        }
        
        protected int? _TechServices_Track_Count = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechServices_Track_Count", "TechServices_Track_Count", DataSource.TABLE, "", false)]
        public virtual int? TechServices_Track_Count
        {
            get
            {
                return _TechServices_Track_Count;
            }
            set
            {
                bool isModify = false;
                if (_TechServices_Track_Count == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechServices_Track_Count.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechServices_Track_Count") == -1)
                    {
                        UPDATE_COLUMN.Add("TechServices_Track_Count");
                    }
                    _TechServices_Track_Count = value;
                }
            }
        }
        
        protected decimal? _TechServices_Track_Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechServices_Track_Price", "TechServices_Track_Price", DataSource.TABLE, "", false)]
        public virtual decimal? TechServices_Track_Price
        {
            get
            {
                return _TechServices_Track_Price;
            }
            set
            {
                bool isModify = false;
                if (_TechServices_Track_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechServices_Track_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechServices_Track_Price") == -1)
                    {
                        UPDATE_COLUMN.Add("TechServices_Track_Price");
                    }
                    _TechServices_Track_Price = value;
                }
            }
        }
        
        protected string _TechServices_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TechServices_Description", "TechServices_Description", DataSource.TABLE, "", false)]
        public virtual string TechServices_Description
        {
            get
            {
                return _TechServices_Description;
            }
            set
            {
                bool isModify = false;
                if (_TechServices_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TechServices_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TechServices_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("TechServices_Description");
                    }
                    _TechServices_Description = value;
                }
            }
        }
        
        protected string _Other_Plan_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Other_Plan_Description", "Other_Plan_Description", DataSource.TABLE, "", false)]
        public virtual string Other_Plan_Description
        {
            get
            {
                return _Other_Plan_Description;
            }
            set
            {
                bool isModify = false;
                if (_Other_Plan_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Other_Plan_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Other_Plan_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Other_Plan_Description");
                    }
                    _Other_Plan_Description = value;
                }
            }
        }
        
        protected string _Other_Track_Description = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Other_Track_Description", "Other_Track_Description", DataSource.TABLE, "", false)]
        public virtual string Other_Track_Description
        {
            get
            {
                return _Other_Track_Description;
            }
            set
            {
                bool isModify = false;
                if (_Other_Track_Description == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Other_Track_Description.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Other_Track_Description") == -1)
                    {
                        UPDATE_COLUMN.Add("Other_Track_Description");
                    }
                    _Other_Track_Description = value;
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
    public partial class OFS_SCI_Outcomes : IOFS_SCI_Outcomes
    {
    }
    
}