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
    /// 科專申請表-申請表主表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_Application_Main", "科專申請表-申請表主表", false)]
    public class IOFS_SCI_Application_Main : IMeta
    {
        
        protected string _Version_ID = null;
        ///<summary>
        /// 補助計畫主鍵 (補助計畫主鍵)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Version_ID", "Version_ID", DataSource.TABLE, "補助計畫主鍵", true)]
        public virtual string Version_ID
        {
            get
            {
                return _Version_ID;
            }
            set
            {
                bool isModify = false;
                if (_Version_ID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Version_ID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Version_ID") == -1)
                    {
                        UPDATE_COLUMN.Add("Version_ID");
                    }
                    _Version_ID = value;
                }
            }
        }
        
        protected string _PersonID = "";
        ///<summary>
        /// 人員表單外鍵 (人員表單外鍵)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PersonID", "PersonID", DataSource.TABLE, "人員表單外鍵", false)]
        public virtual string PersonID
        {
            get
            {
                return _PersonID;
            }
            set
            {
                bool isModify = false;
                if (_PersonID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PersonID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PersonID") == -1)
                    {
                        UPDATE_COLUMN.Add("PersonID");
                    }
                    _PersonID = value;
                }
            }
        }
        
        protected string _KeywordID = "";
        ///<summary>
        /// 關鍵字表單外建 (關鍵字表單外建)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("KeywordID", "KeywordID", DataSource.TABLE, "關鍵字表單外建", false)]
        public virtual string KeywordID
        {
            get
            {
                return _KeywordID;
            }
            set
            {
                bool isModify = false;
                if (_KeywordID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_KeywordID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("KeywordID") == -1)
                    {
                        UPDATE_COLUMN.Add("KeywordID");
                    }
                    _KeywordID = value;
                }
            }
        }
        
        protected int? _Year = null;
        ///<summary>
        /// 年度 (年度)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Year", "Year", DataSource.TABLE, "年度", false)]
        public virtual int? Year
        {
            get
            {
                return _Year;
            }
            set
            {
                bool isModify = false;
                if (_Year == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Year.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Year") == -1)
                    {
                        UPDATE_COLUMN.Add("Year");
                    }
                    _Year = value;
                }
            }
        }
        
        protected string _Serial = "";
        ///<summary>
        /// 流水序號 (流水序號)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Serial", "Serial", DataSource.TABLE, "流水序號", false)]
        public virtual string Serial
        {
            get
            {
                return _Serial;
            }
            set
            {
                bool isModify = false;
                if (_Serial == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Serial.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Serial") == -1)
                    {
                        UPDATE_COLUMN.Add("Serial");
                    }
                    _Serial = value;
                }
            }
        }
        
        protected string _SubsidyPlanType = "";
        ///<summary>
        /// 補助計畫類別 (補助計畫類別)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SubsidyPlanType", "SubsidyPlanType", DataSource.TABLE, "補助計畫類別", false)]
        public virtual string SubsidyPlanType
        {
            get
            {
                return _SubsidyPlanType;
            }
            set
            {
                bool isModify = false;
                if (_SubsidyPlanType == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SubsidyPlanType.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SubsidyPlanType") == -1)
                    {
                        UPDATE_COLUMN.Add("SubsidyPlanType");
                    }
                    _SubsidyPlanType = value;
                }
            }
        }
        
        protected string _ProjectNameTw = "";
        ///<summary>
        /// 中文計畫名稱 (中文計畫名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProjectNameTw", "ProjectNameTw", DataSource.TABLE, "中文計畫名稱", false)]
        public virtual string ProjectNameTw
        {
            get
            {
                return _ProjectNameTw;
            }
            set
            {
                bool isModify = false;
                if (_ProjectNameTw == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ProjectNameTw.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ProjectNameTw") == -1)
                    {
                        UPDATE_COLUMN.Add("ProjectNameTw");
                    }
                    _ProjectNameTw = value;
                }
            }
        }
        
        protected string _ProjectNameEn = "";
        ///<summary>
        /// 英文計畫名稱 (英文計畫名稱)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProjectNameEn", "ProjectNameEn", DataSource.TABLE, "英文計畫名稱", false)]
        public virtual string ProjectNameEn
        {
            get
            {
                return _ProjectNameEn;
            }
            set
            {
                bool isModify = false;
                if (_ProjectNameEn == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ProjectNameEn.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ProjectNameEn") == -1)
                    {
                        UPDATE_COLUMN.Add("ProjectNameEn");
                    }
                    _ProjectNameEn = value;
                }
            }
        }
        
        protected string _OrgCategory = "";
        ///<summary>
        /// 申請機構類別 (申請機構類別)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OrgCategory", "OrgCategory", DataSource.TABLE, "申請機構類別", false)]
        public virtual string OrgCategory
        {
            get
            {
                return _OrgCategory;
            }
            set
            {
                bool isModify = false;
                if (_OrgCategory == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_OrgCategory.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OrgCategory") == -1)
                    {
                        UPDATE_COLUMN.Add("OrgCategory");
                    }
                    _OrgCategory = value;
                }
            }
        }
        
        protected string _Topic = "";
        ///<summary>
        /// 主題 (主題)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Topic", "Topic", DataSource.TABLE, "主題", false)]
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
        
        protected string _Field = "";
        ///<summary>
        /// 領域 (領域)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Field", "Field", DataSource.TABLE, "領域", false)]
        public virtual string Field
        {
            get
            {
                return _Field;
            }
            set
            {
                bool isModify = false;
                if (_Field == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Field.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Field") == -1)
                    {
                        UPDATE_COLUMN.Add("Field");
                    }
                    _Field = value;
                }
            }
        }
        
        protected bool? _CountryTech_Underwater = false;
        ///<summary>
        /// 水下研究 (水下研究)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CountryTech_Underwater", "CountryTech_Underwater", DataSource.TABLE, "水下研究", false)]
        public virtual bool? CountryTech_Underwater
        {
            get
            {
                return _CountryTech_Underwater;
            }
            set
            {
                bool isModify = false;
                if (_CountryTech_Underwater == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CountryTech_Underwater.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CountryTech_Underwater") == -1)
                    {
                        UPDATE_COLUMN.Add("CountryTech_Underwater");
                    }
                    _CountryTech_Underwater = value;
                }
            }
        }
        
        protected bool? _CountryTech_Geology = false;
        ///<summary>
        /// 海洋調查 (海洋調查)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CountryTech_Geology", "CountryTech_Geology", DataSource.TABLE, "海洋調查", false)]
        public virtual bool? CountryTech_Geology
        {
            get
            {
                return _CountryTech_Geology;
            }
            set
            {
                bool isModify = false;
                if (_CountryTech_Geology == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CountryTech_Geology.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CountryTech_Geology") == -1)
                    {
                        UPDATE_COLUMN.Add("CountryTech_Geology");
                    }
                    _CountryTech_Geology = value;
                }
            }
        }
        
        protected bool? _CountryTech_Physics = false;
        ///<summary>
        /// 海洋物理 (海洋物理)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CountryTech_Physics", "CountryTech_Physics", DataSource.TABLE, "海洋物理", false)]
        public virtual bool? CountryTech_Physics
        {
            get
            {
                return _CountryTech_Physics;
            }
            set
            {
                bool isModify = false;
                if (_CountryTech_Physics == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CountryTech_Physics.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CountryTech_Physics") == -1)
                    {
                        UPDATE_COLUMN.Add("CountryTech_Physics");
                    }
                    _CountryTech_Physics = value;
                }
            }
        }
        
        protected string _OrgName = "";
        ///<summary>
        /// 申請單位 (申請單位)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OrgName", "OrgName", DataSource.TABLE, "申請單位", false)]
        public virtual string OrgName
        {
            get
            {
                return _OrgName;
            }
            set
            {
                bool isModify = false;
                if (_OrgName == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_OrgName.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OrgName") == -1)
                    {
                        UPDATE_COLUMN.Add("OrgName");
                    }
                    _OrgName = value;
                }
            }
        }
        
        protected string _RegisteredAddress = "";
        ///<summary>
        /// 登記地址 (登記地址)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("RegisteredAddress", "RegisteredAddress", DataSource.TABLE, "登記地址", false)]
        public virtual string RegisteredAddress
        {
            get
            {
                return _RegisteredAddress;
            }
            set
            {
                bool isModify = false;
                if (_RegisteredAddress == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_RegisteredAddress.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("RegisteredAddress") == -1)
                    {
                        UPDATE_COLUMN.Add("RegisteredAddress");
                    }
                    _RegisteredAddress = value;
                }
            }
        }
        
        protected string _CorrespondenceAddress = "";
        ///<summary>
        /// 通訊地址 (通訊地址)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CorrespondenceAddress", "CorrespondenceAddress", DataSource.TABLE, "通訊地址", false)]
        public virtual string CorrespondenceAddress
        {
            get
            {
                return _CorrespondenceAddress;
            }
            set
            {
                bool isModify = false;
                if (_CorrespondenceAddress == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_CorrespondenceAddress.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("CorrespondenceAddress") == -1)
                    {
                        UPDATE_COLUMN.Add("CorrespondenceAddress");
                    }
                    _CorrespondenceAddress = value;
                }
            }
        }
        
        protected string _Target = "";
        ///<summary>
        /// 計畫目標 (計畫目標)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Target", "Target", DataSource.TABLE, "計畫目標", false)]
        public virtual string Target
        {
            get
            {
                return _Target;
            }
            set
            {
                bool isModify = false;
                if (_Target == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Target.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Target") == -1)
                    {
                        UPDATE_COLUMN.Add("Target");
                    }
                    _Target = value;
                }
            }
        }
        
        protected string _Summary = "";
        ///<summary>
        /// 計畫內容摘要 (計畫內容摘要)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Summary", "Summary", DataSource.TABLE, "計畫內容摘要", false)]
        public virtual string Summary
        {
            get
            {
                return _Summary;
            }
            set
            {
                bool isModify = false;
                if (_Summary == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Summary.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Summary") == -1)
                    {
                        UPDATE_COLUMN.Add("Summary");
                    }
                    _Summary = value;
                }
            }
        }
        
        protected string _Innovation = "";
        ///<summary>
        /// 創新重點 (創新重點)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Innovation", "Innovation", DataSource.TABLE, "創新重點", false)]
        public virtual string Innovation
        {
            get
            {
                return _Innovation;
            }
            set
            {
                bool isModify = false;
                if (_Innovation == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Innovation.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Innovation") == -1)
                    {
                        UPDATE_COLUMN.Add("Innovation");
                    }
                    _Innovation = value;
                }
            }
        }
        
        protected bool? _Declaration = false;
        ///<summary>
        /// 聲明同意書 (聲明同意書)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Declaration", "Declaration", DataSource.TABLE, "聲明同意書", false)]
        public virtual bool? Declaration
        {
            get
            {
                return _Declaration;
            }
            set
            {
                bool isModify = false;
                if (_Declaration == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Declaration.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Declaration") == -1)
                    {
                        UPDATE_COLUMN.Add("Declaration");
                    }
                    _Declaration = value;
                }
            }
        }
        
        protected bool? _IsRecused = false;
        ///<summary>
        /// 是否有勾選迴避委員 (是否有勾選迴避委員)
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsRecused", "IsRecused", DataSource.TABLE, "是否有勾選迴避委員", false)]
        public virtual bool? IsRecused
        {
            get
            {
                return _IsRecused;
            }
            set
            {
                bool isModify = false;
                if (_IsRecused == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_IsRecused.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("IsRecused") == -1)
                    {
                        UPDATE_COLUMN.Add("IsRecused");
                    }
                    _IsRecused = value;
                }
            }
        }
        
        protected DateTime? _StartTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("StartTime", "StartTime", DataSource.TABLE, "", false)]
        public virtual DateTime? StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                bool isModify = false;
                if (_StartTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_StartTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("StartTime") == -1)
                    {
                        UPDATE_COLUMN.Add("StartTime");
                    }
                    _StartTime = value;
                }
            }
        }
        
        protected DateTime? _EndTime = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("EndTime", "EndTime", DataSource.TABLE, "", false)]
        public virtual DateTime? EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                bool isModify = false;
                if (_EndTime == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_EndTime.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("EndTime") == -1)
                    {
                        UPDATE_COLUMN.Add("EndTime");
                    }
                    _EndTime = value;
                }
            }
        }
        
        protected DateTime? _created_at = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("created_at", "created_at", DataSource.TABLE, "", false)]
        public virtual DateTime? created_at
        {
            get
            {
                return _created_at;
            }
            set
            {
                bool isModify = false;
                if (_created_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_created_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("created_at") == -1)
                    {
                        UPDATE_COLUMN.Add("created_at");
                    }
                    _created_at = value;
                }
            }
        }
        
        protected DateTime? _updated_at = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("updated_at", "updated_at", DataSource.TABLE, "", false)]
        public virtual DateTime? updated_at
        {
            get
            {
                return _updated_at;
            }
            set
            {
                bool isModify = false;
                if (_updated_at == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_updated_at.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("updated_at") == -1)
                    {
                        UPDATE_COLUMN.Add("updated_at");
                    }
                    _updated_at = value;
                }
            }
        }
        
        protected bool? _isExist = true;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("isExist", "isExist", DataSource.TABLE, "", false)]
        public virtual bool? isExist
        {
            get
            {
                return _isExist;
            }
            set
            {
                bool isModify = false;
                if (_isExist == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_isExist.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("isExist") == -1)
                    {
                        UPDATE_COLUMN.Add("isExist");
                    }
                    _isExist = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專申請表-申請表主表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_Application_Main : IOFS_SCI_Application_Main
    {
    }
    
}