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
    [GisTableAttribute("OFS_SCI_PersonnelCost_AbroadTrip", "", false)]
    public class IOFS_SCI_PersonnelCost_AbroadTrip : IMeta
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
        
        protected string _Topic = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Topic", "Topic", DataSource.TABLE, "", false)]
        public virtual string Topic
        {
            get
            {
                return _Topic;
            }
            set
            {
                bool isModify = false;
                if (_Topic == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Topic.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Topic") == -1)
                    {
                        UPDATE_COLUMN.Add("Topic");
                    }
                    _Topic = value;
                }
            }
        }
        
        protected int? _Day = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Day", "Day", DataSource.TABLE, "", false)]
        public virtual int? Day
        {
            get
            {
                return _Day;
            }
            set
            {
                bool isModify = false;
                if (_Day == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Day.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Day") == -1)
                    {
                        UPDATE_COLUMN.Add("Day");
                    }
                    _Day = value;
                }
            }
        }
        
        protected int? _People = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("People", "People", DataSource.TABLE, "", false)]
        public virtual int? People
        {
            get
            {
                return _People;
            }
            set
            {
                bool isModify = false;
                if (_People == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_People.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("People") == -1)
                    {
                        UPDATE_COLUMN.Add("People");
                    }
                    _People = value;
                }
            }
        }
        
        protected double? _TravelPrice = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TravelPrice", "TravelPrice", DataSource.TABLE, "", false)]
        public virtual double? TravelPrice
        {
            get
            {
                return _TravelPrice;
            }
            set
            {
                bool isModify = false;
                if (_TravelPrice == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TravelPrice.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TravelPrice") == -1)
                    {
                        UPDATE_COLUMN.Add("TravelPrice");
                    }
                    _TravelPrice = value;
                }
            }
        }
        
        protected double? _LivingPrice = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("LivingPrice", "LivingPrice", DataSource.TABLE, "", false)]
        public virtual double? LivingPrice
        {
            get
            {
                return _LivingPrice;
            }
            set
            {
                bool isModify = false;
                if (_LivingPrice == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_LivingPrice.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("LivingPrice") == -1)
                    {
                        UPDATE_COLUMN.Add("LivingPrice");
                    }
                    _LivingPrice = value;
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
    public partial class OFS_SCI_PersonnelCost_AbroadTrip : IOFS_SCI_PersonnelCost_AbroadTrip
    {
    }
    
}