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
    /// 科專類-人事經費 (國內差旅費)
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_PersonnelCost_TripForm", "科專類-人事經費", false)]
    public class IOFS_SCI_PersonnelCost_TripForm : IMeta
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
        
        protected string _TripReason = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TripReason", "TripReason", DataSource.TABLE, "", false)]
        public virtual string TripReason
        {
            get
            {
                return _TripReason;
            }
            set
            {
                bool isModify = false;
                if (_TripReason == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TripReason.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TripReason") == -1)
                    {
                        UPDATE_COLUMN.Add("TripReason");
                    }
                    _TripReason = value;
                }
            }
        }
        
        protected string _Area = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Area", "Area", DataSource.TABLE, "", false)]
        public virtual string Area
        {
            get
            {
                return _Area;
            }
            set
            {
                bool isModify = false;
                if (_Area == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Area.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Area") == -1)
                    {
                        UPDATE_COLUMN.Add("Area");
                    }
                    _Area = value;
                }
            }
        }
        
        protected decimal? _Days = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Days", "Days", DataSource.TABLE, "", false)]
        public virtual decimal? Days
        {
            get
            {
                return _Days;
            }
            set
            {
                bool isModify = false;
                if (_Days == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Days.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Days") == -1)
                    {
                        UPDATE_COLUMN.Add("Days");
                    }
                    _Days = value;
                }
            }
        }
        
        protected decimal? _Times = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Times", "Times", DataSource.TABLE, "", false)]
        public virtual decimal? Times
        {
            get
            {
                return _Times;
            }
            set
            {
                bool isModify = false;
                if (_Times == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Times.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Times") == -1)
                    {
                        UPDATE_COLUMN.Add("Times");
                    }
                    _Times = value;
                }
            }
        }
        
        protected decimal? _Price = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Price", "Price", DataSource.TABLE, "", false)]
        public virtual decimal? Price
        {
            get
            {
                return _Price;
            }
            set
            {
                bool isModify = false;
                if (_Price == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Price.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Price") == -1)
                    {
                        UPDATE_COLUMN.Add("Price");
                    }
                    _Price = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專類-人事經費 (國內差旅費)
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_PersonnelCost_TripForm : IOFS_SCI_PersonnelCost_TripForm
    {
    }
    
}