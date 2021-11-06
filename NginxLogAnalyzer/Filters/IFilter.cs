using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Filters
{
    internal interface IFilter : ISetting
    {
        bool Matches(AccessEntry entry);

        FilterGroups Group { get; }
    }
}
