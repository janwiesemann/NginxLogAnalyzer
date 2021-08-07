using System;
using System.Globalization;

namespace NginxLogAnalyzer.Filter
{
    class AccessEntryFilterAccessTime : AccessEntryValueFilterBase<DateTime?>
    {
        public AccessEntryFilterAccessTime() : base("accessTime")
        { }

        public override bool Matches(AccessEntry entry)
        {
            if (Value == null)
                return false;

            return entry.DateTime >= Value.Value;
        }

        protected override DateTime? ConvertToValue(string valStr)
        {
            if (DateTime.TryParseExact(valStr, "dd.MM.yyyy-hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime date))
                return date;

            return null;
        }
    }
}
