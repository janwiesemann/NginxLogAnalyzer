using System;
using System.Text.RegularExpressions;

namespace NginxLogAnalyzer.Filters
{
    internal abstract class RegexFilterBase : FilterBase<string>
    {
        private Regex regex;

        public RegexFilterBase(string paramName) : base(paramName)
        { }

        public sealed override bool Matches(AccessEntry entry)
        {
            if (regex == null)
                return true;

            string val = GetPropertyValue(entry);
            if (val == null)
                return true;

            return IsMatch(regex, val);
        }

        protected virtual bool IsMatch(Regex regex, string val)
        {
            return regex.IsMatch(val);
        }

        protected abstract string GetPropertyValue(AccessEntry entry);

        protected virtual RegexOptions GetOptions()
        {
            return RegexOptions.IgnoreCase;
        }

        protected sealed override bool TryParse(string value, out string res)
        {
            try
            {
                regex = new Regex(value, GetOptions());

                res = value;
                return true;
            }
            catch (Exception)
            {
                regex = null;

                res = null;
                return false;
            }
        }
    }
}
