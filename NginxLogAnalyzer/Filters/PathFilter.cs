namespace NginxLogAnalyzer.Filters
{
    internal class PathFilter : RegexFilterBase
    {
        protected PathFilter(string paramName) : base(paramName)
        { }

        public PathFilter() : this("path")
        { }

        protected override string GetPropertyValue(AccessEntry entry) => entry?.Request?.URI;
    }
}
