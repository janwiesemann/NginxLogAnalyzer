using System;

namespace NginxLogAnalyzer.Filters
{
    internal class FieldValueFilter : FilterBase<string>
    {
        private readonly Func<AccessEntry, string> fieldSelect;

        public FieldValueFilter(string paramName, FilterGroups group, Func<AccessEntry, string> fieldSelect) : base(paramName)
        {
            Group = group;
            this.fieldSelect = fieldSelect;
        }

        public override FilterGroups Group { get; }

        public override bool Matches(AccessEntry entry)
        {
            string field = fieldSelect(entry);

            return field == Value;
        }

        protected override bool TryParse(string value, out string res)
        {
            res = value;

            return true;
        }
    }
}
