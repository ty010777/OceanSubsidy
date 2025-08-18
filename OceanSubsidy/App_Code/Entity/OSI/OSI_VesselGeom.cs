using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;


namespace GS.OCA_OceanSubsidy.Entity.Base
{

    /// <summary>
    /// 研究船風險檢核圖徵表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_VesselGeom", "研究船風險檢核圖徵表", false)]
    public class IOSI_VesselGeom : IMeta
    {

        protected string _GeomID = Guid.NewGuid().ToString().ToUpper();
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("GeomID", "GeomID", DataSource.TABLE, "", false)]
        public virtual string GeomID
        {
            get
            {
                return _GeomID;
            }
            set
            {
                bool isModify = false;
                if (_GeomID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_GeomID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("GeomID") == -1)
                    {
                        UPDATE_COLUMN.Add("GeomID");
                    }
                    _GeomID = value;
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

        protected string _GeomName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("GeomName", "GeomName", DataSource.TABLE, "", false)]
        public virtual string GeomName
        {
            get
            {
                return _GeomName;
            }
            set
            {
                bool isModify = false;
                if (_GeomName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_GeomName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("GeomName") == -1)
                    {
                        UPDATE_COLUMN.Add("GeomName");
                    }
                    _GeomName = value;
                }
            }
        }

        protected string _GeoData = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("GeoData", "GeoData", DataSource.TABLE, "", false)]
        public virtual string GeoData
        {
            get
            {
                return _GeoData;
            }
            set
            {
                bool isModify = false;
                if (_GeoData == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_GeoData.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("GeoData") == -1)
                    {
                        UPDATE_COLUMN.Add("GeoData");
                    }
                    _GeoData = value;
                }
            }
        }

        protected bool _IsValid = true;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "", false)]
        public virtual bool IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                bool isModify = false;
                if (_IsValid == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_IsValid.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsValid") == -1)
                    {
                        UPDATE_COLUMN.Add("IsValid");
                    }
                    _IsValid = value;
                }
            }
        }

        protected DateTime _CreatedAt = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreatedAt", "CreatedAt", DataSource.TABLE, "", false)]
        public virtual DateTime CreatedAt
        {
            get
            {
                return _CreatedAt;
            }
            set
            {
                bool isModify = false;
                if (_CreatedAt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CreatedAt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreatedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("CreatedAt");
                    }
                    _CreatedAt = value;
                }
            }
        }

        protected DateTime? _DeletedAt = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DeletedAt", "DeletedAt", DataSource.TABLE, "", false)]
        public virtual DateTime? DeletedAt
        {
            get
            {
                return _DeletedAt;
            }
            set
            {
                bool isModify = false;
                if (_DeletedAt == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_DeletedAt.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DeletedAt") == -1)
                    {
                        UPDATE_COLUMN.Add("DeletedAt");
                    }
                    _DeletedAt = value;
                }
            }
        }

        protected string _DeletedBy = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("DeletedBy", "DeletedBy", DataSource.TABLE, "", false)]
        public virtual string DeletedBy
        {
            get
            {
                return _DeletedBy;
            }
            set
            {
                bool isModify = false;
                if (_DeletedBy == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_DeletedBy.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("DeletedBy") == -1)
                    {
                        UPDATE_COLUMN.Add("DeletedBy");
                    }
                    _DeletedBy = value;
                }
            }
        }

    }




}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 研究船風險檢核圖徵表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_VesselGeom : IOSI_VesselGeom
    {
    }

}