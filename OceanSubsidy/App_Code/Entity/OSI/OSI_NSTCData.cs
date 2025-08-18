using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using GS.Data;

namespace GS.OCA_OceanSubsidy.Entity.Base
{

    /// <summary>
    /// 國科會補助研究計畫資料 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OSI_NSTCData", "國科會補助研究計畫資料", false)]
    public class IOSI_NSTCData : IMeta
    {

        protected int _ID = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ID", "ID", DataSource.UN_OPERATE, "", true)]
        public virtual int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                bool isModify = false;
                if (_ID == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ID.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ID") == -1)
                    {
                        UPDATE_COLUMN.Add("ID");
                    }
                    _ID = value;
                }
            }
        }

        protected string _Year = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Year", "Year", DataSource.TABLE, "", false)]
        public virtual string Year
        {
            get
            {
                return _Year;
            }
            set
            {
                bool isModify = false;
                if (_Year == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Year.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Year") == -1)
                    {
                        UPDATE_COLUMN.Add("Year");
                    }
                    _Year = value;
                }
            }
        }

        protected string _Unit = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Unit", "Unit", DataSource.TABLE, "", false)]
        public virtual string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                bool isModify = false;
                if (_Unit == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_Unit.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Unit") == -1)
                    {
                        UPDATE_COLUMN.Add("Unit");
                    }
                    _Unit = value;
                }
            }
        }

        protected string _tName = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("tName", "tName", DataSource.TABLE, "", false)]
        public virtual string tName
        {
            get
            {
                return _tName;
            }
            set
            {
                bool isModify = false;
                if (_tName == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_tName.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("tName") == -1)
                    {
                        UPDATE_COLUMN.Add("tName");
                    }
                    _tName = value;
                }
            }
        }

        protected string _TotalApprovedAmount = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("TotalApprovedAmount", "TotalApprovedAmount", DataSource.TABLE, "", false)]
        public virtual string TotalApprovedAmount
        {
            get
            {
                return _TotalApprovedAmount;
            }
            set
            {
                bool isModify = false;
                if (_TotalApprovedAmount == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_TotalApprovedAmount.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("TotalApprovedAmount") == -1)
                    {
                        UPDATE_COLUMN.Add("TotalApprovedAmount");
                    }
                    _TotalApprovedAmount = value;
                }
            }
        }

        protected DateTime? _ExecutionStart = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExecutionStart", "ExecutionStart", DataSource.TABLE, "", false)]
        public virtual DateTime? ExecutionStart
        {
            get
            {
                return _ExecutionStart;
            }
            set
            {
                bool isModify = false;
                if (_ExecutionStart == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ExecutionStart.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ExecutionStart") == -1)
                    {
                        UPDATE_COLUMN.Add("ExecutionStart");
                    }
                    _ExecutionStart = value;
                }
            }
        }

        protected DateTime? _ExecutionEnd = null;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("ExecutionEnd", "ExecutionEnd", DataSource.TABLE, "", false)]
        public virtual DateTime? ExecutionEnd
        {
            get
            {
                return _ExecutionEnd;
            }
            set
            {
                bool isModify = false;
                if (_ExecutionEnd == null)
                {
                    if (value != null)
                    {
                        isModify = true;
                    }
                }
                else if (!_ExecutionEnd.Equals(value))
                {
                    isModify = true;
                }
                if (isModify)
                {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("ExecutionEnd") == -1)
                    {
                        UPDATE_COLUMN.Add("ExecutionEnd");
                    }
                    _ExecutionEnd = value;
                }
            }
        }

        protected DateTime? _CreateDate = DateTime.Now;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("CreateDate", "CreateDate", DataSource.TABLE, "", false)]
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
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("UpdateDate", "UpdateDate", DataSource.TABLE, "", false)]
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
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("IsValid", "IsValid", DataSource.TABLE, "", false)]
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
    /// 國科會補助研究計畫資料 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OSI_NSTCData : IOSI_NSTCData
    {
    }

}