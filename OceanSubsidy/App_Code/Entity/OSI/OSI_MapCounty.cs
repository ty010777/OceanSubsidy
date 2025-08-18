using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 各縣市圖資 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_MapCounty", "各縣市圖資", false)]
    public class IOSI_MapCounty : IMeta
    {

        protected int _qgs_fid = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("qgs_fid", "qgs_fid", DataSource.TABLE, "", false)]
        public virtual int qgs_fid
        {
            get
            {
                return _qgs_fid;
            }
            set
            {
                bool isModify = false;
                if (_qgs_fid == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_qgs_fid.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("qgs_fid") == -1)
                    {
                        UPDATE_COLUMN.Add("qgs_fid");
                    }
                    _qgs_fid = value;
                }
            }
        }

        protected string _geom = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("geom", "geom", DataSource.TABLE, "", false)]
        public virtual string geom
        {
            get
            {
                return _geom;
            }
            set
            {
                bool isModify = false;
                if (_geom == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_geom.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("geom") == -1)
                    {
                        UPDATE_COLUMN.Add("geom");
                    }
                    _geom = value;
                }
            }
        }

        protected int? _ogr_fid = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ogr_fid", "ogr_fid", DataSource.TABLE, "", false)]
        public virtual int? ogr_fid
        {
            get
            {
                return _ogr_fid;
            }
            set
            {
                bool isModify = false;
                if (_ogr_fid == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ogr_fid.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ogr_fid") == -1)
                    {
                        UPDATE_COLUMN.Add("ogr_fid");
                    }
                    _ogr_fid = value;
                }
            }
        }

        protected string _county_id = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("county_id", "county_id", DataSource.TABLE, "", false)]
        public virtual string county_id
        {
            get
            {
                return _county_id;
            }
            set
            {
                bool isModify = false;
                if (_county_id == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_county_id.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("county_id") == -1)
                    {
                        UPDATE_COLUMN.Add("county_id");
                    }
                    _county_id = value;
                }
            }
        }

        protected string _c_name = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("c_name", "c_name", DataSource.TABLE, "", false)]
        public virtual string c_name
        {
            get
            {
                return _c_name;
            }
            set
            {
                bool isModify = false;
                if (_c_name == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_c_name.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("c_name") == -1)
                    {
                        UPDATE_COLUMN.Add("c_name");
                    }
                    _c_name = value;
                }
            }
        }

        protected string _c_desc = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("c_desc", "c_desc", DataSource.TABLE, "", false)]
        public virtual string c_desc
        {
            get
            {
                return _c_desc;
            }
            set
            {
                bool isModify = false;
                if (_c_desc == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_c_desc.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("c_desc") == -1)
                    {
                        UPDATE_COLUMN.Add("c_desc");
                    }
                    _c_desc = value;
                }
            }
        }

        protected DateTime? _add_date = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("add_date", "add_date", DataSource.TABLE, "", false)]
        public virtual DateTime? add_date
        {
            get
            {
                return _add_date;
            }
            set
            {
                bool isModify = false;
                if (_add_date == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_add_date.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("add_date") == -1)
                    {
                        UPDATE_COLUMN.Add("add_date");
                    }
                    _add_date = value;
                }
            }
        }

        protected string _add_accept = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("add_accept", "add_accept", DataSource.TABLE, "", false)]
        public virtual string add_accept
        {
            get
            {
                return _add_accept;
            }
            set
            {
                bool isModify = false;
                if (_add_accept == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_add_accept.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("add_accept") == -1)
                    {
                        UPDATE_COLUMN.Add("add_accept");
                    }
                    _add_accept = value;
                }
            }
        }

        protected string _remark = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("remark", "remark", DataSource.TABLE, "", false)]
        public virtual string remark
        {
            get
            {
                return _remark;
            }
            set
            {
                bool isModify = false;
                if (_remark == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_remark.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("remark") == -1)
                    {
                        UPDATE_COLUMN.Add("remark");
                    }
                    _remark = value;
                }
            }
        }

        protected int? _orderBy = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("orderBy", "orderBy", DataSource.TABLE, "", false)]
        public virtual int? orderBy
        {
            get
            {
                return _orderBy;
            }
            set
            {
                bool isModify = false;
                if (_orderBy == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_orderBy.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("orderBy") == -1)
                    {
                        UPDATE_COLUMN.Add("orderBy");
                    }
                    _orderBy = value;
                }
            }
        }


    }



}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;

    /// <summary>
    /// 各縣市圖資 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_MapCounty : IOSI_MapCounty
    {
    }

}

