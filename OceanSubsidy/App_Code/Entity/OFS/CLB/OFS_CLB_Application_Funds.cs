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
    [GisTableAttribute("OFS_CLB_Application_Funds", "", false)]
    public class IOFS_CLB_Application_Funds : IMeta
    {
        
        protected string _ProjectID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ProjectID", "ProjectID", DataSource.TABLE, "", false)]
        public virtual string ProjectID
        {
            get
            {
                return _ProjectID;
            }
            set
            {
                bool isModify = false;
                if (_ProjectID == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_ProjectID.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ProjectID") == -1)
                    {
                        UPDATE_COLUMN.Add("ProjectID");
                    }
                    _ProjectID = value;
                }
            }
        }
        
        protected decimal? _SubsidyFunds = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SubsidyFunds", "SubsidyFunds", DataSource.TABLE, "", false)]
        public virtual decimal? SubsidyFunds
        {
            get
            {
                return _SubsidyFunds;
            }
            set
            {
                bool isModify = false;
                if (_SubsidyFunds == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SubsidyFunds.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SubsidyFunds") == -1)
                    {
                        UPDATE_COLUMN.Add("SubsidyFunds");
                    }
                    _SubsidyFunds = value;
                }
            }
        }
        
        protected decimal? _SelfFunds = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("SelfFunds", "SelfFunds", DataSource.TABLE, "", false)]
        public virtual decimal? SelfFunds
        {
            get
            {
                return _SelfFunds;
            }
            set
            {
                bool isModify = false;
                if (_SelfFunds == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_SelfFunds.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("SelfFunds") == -1)
                    {
                        UPDATE_COLUMN.Add("SelfFunds");
                    }
                    _SelfFunds = value;
                }
            }
        }
        
        protected decimal? _OtherGovFunds = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OtherGovFunds", "OtherGovFunds", DataSource.TABLE, "", false)]
        public virtual decimal? OtherGovFunds
        {
            get
            {
                return _OtherGovFunds;
            }
            set
            {
                bool isModify = false;
                if (_OtherGovFunds == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_OtherGovFunds.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OtherGovFunds") == -1)
                    {
                        UPDATE_COLUMN.Add("OtherGovFunds");
                    }
                    _OtherGovFunds = value;
                }
            }
        }
        
        protected decimal? _OtherUnitFunds = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("OtherUnitFunds", "OtherUnitFunds", DataSource.TABLE, "", false)]
        public virtual decimal? OtherUnitFunds
        {
            get
            {
                return _OtherUnitFunds;
            }
            set
            {
                bool isModify = false;
                if (_OtherUnitFunds == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_OtherUnitFunds.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("OtherUnitFunds") == -1)
                    {
                        UPDATE_COLUMN.Add("OtherUnitFunds");
                    }
                    _OtherUnitFunds = value;
                }
            }
        }
        
        protected decimal? _TotalFunds = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TotalFunds", "TotalFunds", DataSource.TABLE, "", false)]
        public virtual decimal? TotalFunds
        {
            get
            {
                return _TotalFunds;
            }
            set
            {
                bool isModify = false;
                if (_TotalFunds == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_TotalFunds.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TotalFunds") == -1)
                    {
                        UPDATE_COLUMN.Add("TotalFunds");
                    }
                    _TotalFunds = value;
                }
            }
        }
        
        protected bool? _PreviouslySubsidized = false;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PreviouslySubsidized", "PreviouslySubsidized", DataSource.TABLE, "", false)]
        public virtual bool? PreviouslySubsidized
        {
            get
            {
                return _PreviouslySubsidized;
            }
            set
            {
                bool isModify = false;
                if (_PreviouslySubsidized == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PreviouslySubsidized.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PreviouslySubsidized") == -1)
                    {
                        UPDATE_COLUMN.Add("PreviouslySubsidized");
                    }
                    _PreviouslySubsidized = value;
                }
            }
        }
        
        protected string _FundingDescription = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("FundingDescription", "FundingDescription", DataSource.TABLE, "", false)]
        public virtual string FundingDescription
        {
            get
            {
                return _FundingDescription;
            }
            set
            {
                bool isModify = false;
                if (_FundingDescription == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_FundingDescription.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("FundingDescription") == -1)
                    {
                        UPDATE_COLUMN.Add("FundingDescription");
                    }
                    _FundingDescription = value;
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
    public partial class OFS_CLB_Application_Funds : IOFS_CLB_Application_Funds
    {
    }
    
}