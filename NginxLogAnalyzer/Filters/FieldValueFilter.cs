using System;

namespace NginxLogAnalyzer.Filters
{
    internal class FieldValueFilter : FilterBase<string>
    {
        private readonly Func<AccessEntry, string> fieldSelect;

        public FieldValueFilter(string paramName, Func<AccessEntry, string> fieldSelect) : base(paramName)
        {
            this.fieldSelect = fieldSelect;
        }

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
