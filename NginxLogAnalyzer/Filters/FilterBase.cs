using NginxLogAnalyzer.Settings;

namespace NginxLogAnalyzer.Filters
{
    internal abstract class FilterBase<T> : SettingBase<T>, IFilter
    {
        public FilterBase(string paramName) : base(paramName) 
        { }

        public abstract bool Matches(AccessEntry entry);
    }
}
