using System;

namespace NginxLogAnalyzer.Filter
{
    class AccessEntryFilterFieldMatchesValue : AccessEntryValueFilterBase<string>
    {
        private readonly Func<AccessEntry, object> fieldSelect;

        public AccessEntryFilterFieldMatchesValue(string paramName, Func<AccessEntry, object> fieldSelect) : base(paramName)
        {
            this.fieldSelect = fieldSelect;
        }

        public override bool Matches(AccessEntry entry)
        {
            object obj = fieldSelect(entry);

            return obj.ToString() == Value;
        }

        protected override string ConvertToValue(string valStr) => valStr;
    }
}
