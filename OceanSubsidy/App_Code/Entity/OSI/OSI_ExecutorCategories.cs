using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// OSI活動執行者種類 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_ExecutorCategories", "OSI活動執行者種類", false)]
    public class IOSI_ExecutorCategories : IMeta
    {

        protected int _CategoryID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CategoryID", "CategoryID", DataSource.UN_OPERATE, "", true)]
        public virtual int CategoryID
        {
            get
            {
                return _CategoryID;
            }
            set
            {
                bool isModify = false;
                if (_CategoryID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CategoryID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CategoryID") == -1)
                    {
                        UPDATE_COLUMN.Add("CategoryID");
                    }
                    _CategoryID = value;
                }
            }
        }

        protected string _CategoryName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CategoryName", "CategoryName", DataSource.TABLE, "", false)]
        public virtual string CategoryName
        {
            get
            {
                return _CategoryName;
            }
            set
            {
                bool isModify = false;
                if (_CategoryName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CategoryName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CategoryName") == -1)
                    {
                        UPDATE_COLUMN.Add("CategoryName");
                    }
                    _CategoryName = value;
                }
            }
        }

    }

}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    /// <summary>
    /// OSI活動執行者種類 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_ExecutorCategories : IOSI_ExecutorCategories
    {
    }

}
