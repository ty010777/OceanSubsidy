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
    /// 科專申請表關鍵字表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    [GisTableAttribute("OFS_SCI_Application_KeyWord", "科專申請表關鍵字表", false)]
    public class IOFS_SCI_Application_KeyWord : IMeta
    {
        
        protected int _Idx = 0;
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("Idx", "Idx", DataSource.UN_OPERATE, "", true)]
        public virtual int Idx
        {
            get
            {
                return _Idx;
            }
            set
            {
                bool isModify = false;
                if (_Idx == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_Idx.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("Idx") == -1)
                    {
                        UPDATE_COLUMN.Add("Idx");
                    }
                    _Idx = value;
                }
            }
        }
        
        protected string _KeywordID = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("KeywordID", "KeywordID", DataSource.TABLE, "", false)]
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
        
        protected string _KeyWordTw = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("KeyWordTw", "KeyWordTw", DataSource.TABLE, "", false)]
        public virtual string KeyWordTw
        {
            get
            {
                return _KeyWordTw;
            }
            set
            {
                bool isModify = false;
                if (_KeyWordTw == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_KeyWordTw.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("KeyWordTw") == -1)
                    {
                        UPDATE_COLUMN.Add("KeyWordTw");
                    }
                    _KeyWordTw = value;
                }
            }
        }
        
        protected string _KeyWordEn = "";
        ///<summary>
        ///  ()
        ///</summary>
        [DataMember]
        [GisFieldAttribute("KeyWordEn", "KeyWordEn", DataSource.TABLE, "", false)]
        public virtual string KeyWordEn
        {
            get
            {
                return _KeyWordEn;
            }
            set
            {
                bool isModify = false;
                if (_KeyWordEn == null) {
                    if(value != null) {
                        isModify = true;
                    }
                }
                else if (!_KeyWordEn.Equals(value))
                {
                    isModify = true;
                }
                if(isModify) {
                    MetaDataState = DataState.UPDATE;
                    if (UPDATE_COLUMN.IndexOf("KeyWordEn") == -1)
                    {
                        UPDATE_COLUMN.Add("KeyWordEn");
                    }
                    _KeyWordEn = value;
                }
            }
        }
        
    }

    
    
    
}

namespace GS.OCA_OceanSubsidy.Entity
{
    using GS.OCA_OceanSubsidy.Entity.Base;
    
    /// <summary>
    /// 科專申請表關鍵字表 ()
    /// </summary>
    [DataContract]
    [Serializable()]
    public partial class OFS_SCI_Application_KeyWord : IOFS_SCI_Application_KeyWord
    {
    }
    
}