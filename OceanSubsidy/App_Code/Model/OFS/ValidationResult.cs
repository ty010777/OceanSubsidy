using System;
using System.Collections.Generic;

namespace GS.OCA_OceanSubsidy.Model.OFS
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; private set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public string GetErrorsAsString()
        {
            return string.Join("\\n", Errors);
        }
    }
}