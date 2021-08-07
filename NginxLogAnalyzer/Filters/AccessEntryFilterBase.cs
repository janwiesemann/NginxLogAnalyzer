using System;

namespace NginxLogAnalyzer.Filter
{
    abstract class AccessEntryFilterBase
    {
        public string ParameterName { get; }

        public bool HasValue { get; protected set; }

        public AccessEntryFilterBase(string paramName)
        {
            ParameterName = paramName;
        }

        public abstract void Parse(string[] args);

        public abstract bool Matches(AccessEntry entry);
    }
}
