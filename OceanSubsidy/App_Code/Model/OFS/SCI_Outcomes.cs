using System.Collections.Generic;

public class OutcomeRequest
{
    public string ProjectID { get; set; }
    public List<OutcomeItem> outcomeData { get; set; }
}

public class OutcomeItem
{
    public string item { get; set; }
    public Dictionary<string, string> values { get; set; }
    public string description { get; set; }
}