using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 風險檢核種類表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_VesselRiskCategory", "風險檢核種類表", false)]
    public class IOSI_VesselRiskCategory : IMeta
    {

        protected int _RiskCategoryId = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RiskCategoryId", "RiskCategoryId", DataSource.UN_OPERATE, "", true)]
        public virtual int RiskCategoryId
        {
            get
            {
                return _RiskCategoryId;
            }
            set
            {
                bool isModify = false;
                if (_RiskCategoryId == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_RiskCategoryId.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("RiskCategoryId") == -1)
                    {
                        UPDATE_COLUMN.Add("RiskCategoryId");
                    }
                    _RiskCategoryId = value;
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
    /// 風險檢核種類表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_VesselRiskCategory : IOSI_VesselRiskCategory
    {
    }


}