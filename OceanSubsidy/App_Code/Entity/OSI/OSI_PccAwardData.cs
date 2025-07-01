using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;


namespace GS.OCA_OceanSubsidy.Entity.Base
{
    /// <summary>
    /// 政府採購網決標資料 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_PccAwardData", "政府採購網決標資料", false)]
    public class IOSI_PccAwardData : IMeta
    {

        protected int _Id = 0;
        ///<summary>
        /// 流水號 (流水號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Id", "Id", DataSource.UN_OPERATE, "流水號", true)]
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

        protected string _OrgName = "";
        ///<summary>
        /// 機關名稱 (機關名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OrgName", "OrgName", DataSource.TABLE, "機關名稱", false)]
        public virtual string OrgName
        {
            get
            {
                return _OrgName;
            }
            set
            {
                bool isModify = false;
                if (_OrgName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_OrgName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OrgName") == -1)
                    {
                        UPDATE_COLUMN.Add("OrgName");
                    }
                    _OrgName = value;
                }
            }
        }

        protected string _AwardNo = "";
        ///<summary>
        /// 標案案號 (標案案號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AwardNo", "AwardNo", DataSource.TABLE, "標案案號", false)]
        public virtual string AwardNo
        {
            get
            {
                return _AwardNo;
            }
            set
            {
                bool isModify = false;
                if (_AwardNo == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AwardNo.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AwardNo") == -1)
                    {
                        UPDATE_COLUMN.Add("AwardNo");
                    }
                    _AwardNo = value;
                }
            }
        }

        protected string _AwardName = "";
        ///<summary>
        /// 標案名稱 (標案名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AwardName", "AwardName", DataSource.TABLE, "標案名稱", false)]
        public virtual string AwardName
        {
            get
            {
                return _AwardName;
            }
            set
            {
                bool isModify = false;
                if (_AwardName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AwardName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AwardName") == -1)
                    {
                        UPDATE_COLUMN.Add("AwardName");
                    }
                    _AwardName = value;
                }
            }
        }

        protected string _AwardNameUrl = "";
        ///<summary>
        /// 標案名稱超連結 (標案名稱超連結)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AwardNameUrl", "AwardNameUrl", DataSource.TABLE, "標案名稱超連結", false)]
        public virtual string AwardNameUrl
        {
            get
            {
                return _AwardNameUrl;
            }
            set
            {
                bool isModify = false;
                if (_AwardNameUrl == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AwardNameUrl.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AwardNameUrl") == -1)
                    {
                        UPDATE_COLUMN.Add("AwardNameUrl");
                    }
                    _AwardNameUrl = value;
                }
            }
        }

        protected string _AwardWay = "";
        ///<summary>
        /// 招標方式 (招標方式)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AwardWay", "AwardWay", DataSource.TABLE, "招標方式", false)]
        public virtual string AwardWay
        {
            get
            {
                return _AwardWay;
            }
            set
            {
                bool isModify = false;
                if (_AwardWay == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AwardWay.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AwardWay") == -1)
                    {
                        UPDATE_COLUMN.Add("AwardWay");
                    }
                    _AwardWay = value;
                }
            }
        }

        protected string _ProctrgCate = "";
        ///<summary>
        /// 標的分類 (標的分類)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProctrgCate", "ProctrgCate", DataSource.TABLE, "標的分類", false)]
        public virtual string ProctrgCate
        {
            get
            {
                return _ProctrgCate;
            }
            set
            {
                bool isModify = false;
                if (_ProctrgCate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ProctrgCate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ProctrgCate") == -1)
                    {
                        UPDATE_COLUMN.Add("ProctrgCate");
                    }
                    _ProctrgCate = value;
                }
            }
        }

        protected DateTime? _AwardDate = null;
        ///<summary>
        /// 公告日期(西元年) (公告日期(西元年))
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AwardDate", "AwardDate", DataSource.TABLE, "公告日期(西元年)", false)]
        public virtual DateTime? AwardDate
        {
            get
            {
                return _AwardDate;
            }
            set
            {
                bool isModify = false;
                if (_AwardDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AwardDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AwardDate") == -1)
                    {
                        UPDATE_COLUMN.Add("AwardDate");
                    }
                    _AwardDate = value;
                }
            }
        }

        protected string _AwardPrice = "";
        ///<summary>
        /// 決標金額 (決標金額)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AwardPrice", "AwardPrice", DataSource.TABLE, "決標金額", false)]
        public virtual string AwardPrice
        {
            get
            {
                return _AwardPrice;
            }
            set
            {
                bool isModify = false;
                if (_AwardPrice == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AwardPrice.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AwardPrice") == -1)
                    {
                        UPDATE_COLUMN.Add("AwardPrice");
                    }
                    _AwardPrice = value;
                }
            }
        }

        protected string _AwardNoticeNo = "";
        ///<summary>
        /// 決標公告編號 (決標公告編號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AwardNoticeNo", "AwardNoticeNo", DataSource.TABLE, "決標公告編號", false)]
        public virtual string AwardNoticeNo
        {
            get
            {
                return _AwardNoticeNo;
            }
            set
            {
                bool isModify = false;
                if (_AwardNoticeNo == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_AwardNoticeNo.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AwardNoticeNo") == -1)
                    {
                        UPDATE_COLUMN.Add("AwardNoticeNo");
                    }
                    _AwardNoticeNo = value;
                }
            }
        }

        protected string _Url = "";
        ///<summary>
        /// 原始URL (原始URL)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Url", "Url", DataSource.TABLE, "原始URL", false)]
        public virtual string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                bool isModify = false;
                if (_Url == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Url.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Url") == -1)
                    {
                        UPDATE_COLUMN.Add("Url");
                    }
                    _Url = value;
                }
            }
        }

        protected string _QueryParams = "";
        ///<summary>
        /// 查詢參數 (查詢參數)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("QueryParams", "QueryParams", DataSource.TABLE, "查詢參數", false)]
        public virtual string QueryParams
        {
            get
            {
                return _QueryParams;
            }
            set
            {
                bool isModify = false;
                if (_QueryParams == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_QueryParams.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("QueryParams") == -1)
                    {
                        UPDATE_COLUMN.Add("QueryParams");
                    }
                    _QueryParams = value;
                }
            }
        }

        protected DateTime? _CreateDate = DateTime.Now;
        ///<summary>
        /// 建立時間 (建立時間)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreateDate", "CreateDate", DataSource.TABLE, "建立時間", false)]
        public virtual DateTime? CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                bool isModify = false;
                if (_CreateDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_CreateDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CreateDate") == -1)
                    {
                        UPDATE_COLUMN.Add("CreateDate");
                    }
                    _CreateDate = value;
                }
            }
        }

        protected DateTime? _UpdateDate = null;
        ///<summary>
        /// 更新時間 (更新時間)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UpdateDate", "UpdateDate", DataSource.TABLE, "更新時間", false)]
        public virtual DateTime? UpdateDate
        {
            get
            {
                return _UpdateDate;
            }
            set
            {
                bool isModify = false;
                if (_UpdateDate == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_UpdateDate.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("UpdateDate") == -1)
                    {
                        UPDATE_COLUMN.Add("UpdateDate");
                    }
                    _UpdateDate = value;
                }
            }
        }

        protected bool? _IsValid = true;
        ///<summary>
        /// 是否刪除 (是否刪除)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "是否刪除", false)]
        public virtual bool? IsValid
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

    }



}


namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    /// <summary>
    /// 政府採購網決標資料 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_PccAwardData : IOSI_PccAwardData
    {
    }



}
