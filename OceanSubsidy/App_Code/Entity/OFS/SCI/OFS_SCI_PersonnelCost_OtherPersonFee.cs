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
    /// 科專類-人事經費 (其他業務費-勞務委託費)
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_PersonnelCost_OtherPersonFee", "科專類-人事經費", false)]
    public class IOFS_SCI_PersonnelCost_OtherPersonFee : IMeta
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
        
        protected string _JobTitle = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("JobTitle", "JobTitle", DataSource.TABLE, "", false)]
        public virtual string JobTitle
        {
            get
            {
                return _JobTitle;
            }
            set
            {
                bool isModify = false;
                if (_JobTitle == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_JobTitle.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("JobTitle") == -1)
                    {
                        UPDATE_COLUMN.Add("JobTitle");
                    }
                    _JobTitle = value;
                }
            }
        }
        
        protected decimal? _AvgSalary = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("AvgSalary", "AvgSalary", DataSource.TABLE, "", false)]
        public virtual decimal? AvgSalary
        {
            get
            {
                return _AvgSalary;
            }
            set
            {
                bool isModify = false;
                if (_AvgSalary == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_AvgSalary.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("AvgSalary") == -1)
                    {
                        UPDATE_COLUMN.Add("AvgSalary");
                    }
                    _AvgSalary = value;
                }
            }
        }
        
        protected decimal? _Month = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Month", "Month", DataSource.TABLE, "", false)]
        public virtual decimal? Month
        {
            get
            {
                return _Month;
            }
            set
            {
                bool isModify = false;
                if (_Month == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Month.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Month") == -1)
                    {
                        UPDATE_COLUMN.Add("Month");
                    }
                    _Month = value;
                }
            }
        }
        
        protected decimal? _PeopleNum = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("PeopleNum", "PeopleNum", DataSource.TABLE, "", false)]
        public virtual decimal? PeopleNum
        {
            get
            {
                return _PeopleNum;
            }
            set
            {
                bool isModify = false;
                if (_PeopleNum == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_PeopleNum.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("PeopleNum") == -1)
                    {
                        UPDATE_COLUMN.Add("PeopleNum");
                    }
                    _PeopleNum = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專類-人事經費 (其他業務費-勞務委託費)
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_PersonnelCost_OtherPersonFee : IOFS_SCI_PersonnelCost_OtherPersonFee
    {
    }
    
}