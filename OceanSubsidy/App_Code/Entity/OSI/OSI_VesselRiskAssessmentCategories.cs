using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{

    /// <summary>
    ///  ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_VesselRiskAssessmentCategories", "", false)]
    public class IOSI_VesselRiskAssessmentCategories : IMeta
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
                if (_Id == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Id.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Id") == -1)
                    {
                        UPDATE_COLUMN.Add("Id");
                    }
                    _Id = value;
                }
            }
        }

        protected int _AssessmentId = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AssessmentId", "AssessmentId", DataSource.TABLE, "", false)]
        public virtual int AssessmentId
        {
            get
            {
                return _AssessmentId;
            }
            set
            {
                bool isModify = false;
                if (_AssessmentId == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AssessmentId.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AssessmentId") == -1)
                    {
                        UPDATE_COLUMN.Add("AssessmentId");
                    }
                    _AssessmentId = value;
                }
            }
        }

        protected int _RiskCategoryId = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RiskCategoryId", "RiskCategoryId", DataSource.TABLE, "", false)]
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
    public partial class OSI_VesselRiskAssessmentCategories : IOSI_VesselRiskAssessmentCategories
    {
    }

}