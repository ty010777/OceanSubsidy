using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 活動性質表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_ActivityNatures", "活動性質表", false)]
    public class IOSI_ActivityNatures : IMeta
    {

        protected int _NatureID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("NatureID", "NatureID", DataSource.UN_OPERATE, "", true)]
        public virtual int NatureID
        {
            get
            {
                return _NatureID;
            }
            set
            {
                bool isModify = false;
                if (_NatureID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_NatureID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("NatureID") == -1)
                    {
                        UPDATE_COLUMN.Add("NatureID");
                    }
                    _NatureID = value;
                }
            }
        }

        protected string _NatureName = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("NatureName", "NatureName", DataSource.TABLE, "", false)]
        public virtual string NatureName
        {
            get
            {
                return _NatureName;
            }
            set
            {
                bool isModify = false;
                if (_NatureName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_NatureName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("NatureName") == -1)
                    {
                        UPDATE_COLUMN.Add("NatureName");
                    }
                    _NatureName = value;
                }
            }
        }

    }

}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 活動性質表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_ActivityNatures : IOSI_ActivityNatures
    {
    }

}