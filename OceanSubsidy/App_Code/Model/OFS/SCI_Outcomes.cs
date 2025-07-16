using System;
using System.Collections.Generic;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    /// <summary>
    /// 成果與績效資料請求類別
    /// </summary>
    public class OutcomeRequest
    {
        public string ProjectID { get; set; }
        public List<OutcomeItem> outcomeData { get; set; }
    }

    /// <summary>
    /// 成果與績效項目類別
    /// </summary>
    public class OutcomeItem
    {
        public string item { get; set; }
        public Dictionary<string, string> values { get; set; }
        public string description { get; set; }
    }
}