using System;
using System.Globalization;

namespace NginxLogAnalyzer.Filters
{
    internal class AccessEntryFilterAccessTime : FilterBase<DateTime>
    {
        public AccessEntryFilterAccessTime() : base("accessTime")
        { }

        public override bool Matches(AccessEntry entry)
        {
            return entry.TimeLocal >= Value;
        }

        protected override bool TryParse(string value, out DateTime res)
        {
            return DateTime.TryParseExact(value, "dd.MM.yyyy-hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out res);
        }
    }
}
