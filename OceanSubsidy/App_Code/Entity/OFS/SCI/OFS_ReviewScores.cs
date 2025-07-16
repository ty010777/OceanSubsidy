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
    [GisTableAttribute("OFS_ReviewScores", "", false)]
    public class IOFS_ReviewScores : IMeta
    {
        
        protected int _Id = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Id", "Id", DataSource.UN_OPERATE, "", true)]
        public virtual int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                bool isModify = false;
                if (_Id == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Id.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Id") == -1)
                    {
                        UPDATE_COLUMN.Add("Id");
                    }
                    _Id = value;
                }
            }
        }
        
        protected int _ReviewID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ReviewID", "ReviewID", DataSource.TABLE, "", false)]
        public virtual int ReviewID
        {
            get
            {
                return _ReviewID;
            }
            set
            {
                bool isModify = false;
                if (_ReviewID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ReviewID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ReviewID") == -1)
                    {
                        UPDATE_COLUMN.Add("ReviewID");
                    }
                    _ReviewID = value;
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
        
        protected double _Weight = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Weight", "Weight", DataSource.TABLE, "", false)]
        public virtual double Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                bool isModify = false;
                if (_Weight == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Weight.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Weight") == -1)
                    {
                        UPDATE_COLUMN.Add("Weight");
                    }
                    _Weight = value;
                }
            }
        }
        
        protected double _Score = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Score", "Score", DataSource.TABLE, "", false)]
        public virtual double Score
        {
            get
            {
                return _Score;
            }
            set
            {
                bool isModify = false;
                if (_Score == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Score.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Score") == -1)
                    {
                        UPDATE_COLUMN.Add("Score");
                    }
                    _Score = value;
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
    public partial class OFS_ReviewScores : IOFS_ReviewScores
    {
    }
    
}