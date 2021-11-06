using System.Text.RegularExpressions;

namespace NginxLogAnalyzer.Filters
{
    internal class NotPathFilter : PathFilter
    {
        public NotPathFilter() : base("npath")
        { }

        protected override bool IsMatch(Regex regex, string val) => !base.IsMatch(regex, val);
    }
}
