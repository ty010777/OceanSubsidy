using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 研究調查項目表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_ResearchItems", "研究調查項目表", false)]
    public class IOSI_ResearchItems : IMeta
    {

        protected int _ItemID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ItemID", "ItemID", DataSource.UN_OPERATE, "", true)]
        public virtual int ItemID
        {
            get
            {
                return _ItemID;
            }
            set
            {
                bool isModify = false;
                if (_ItemID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ItemID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ItemID") == -1)
                    {
                        UPDATE_COLUMN.Add("ItemID");
                    }
                    _ItemID = value;
                }
            }
        }

        protected string _ItemName = null;
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
                if (_ItemName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ItemName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ItemName") == -1)
                    {
                        UPDATE_COLUMN.Add("ItemName");
                    }
                    _ItemName = value;
                }
            }
        }

    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 研究調查項目表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_ResearchItems : IOSI_ResearchItems
    {
    }

}