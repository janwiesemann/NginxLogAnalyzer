namespace NginxLogAnalyzer.Filters
{
    /// <summary>
    /// Filter groups are used to group filters. This can be seen as a And and Or group. For example:
    /// Group 1
    /// -> Filter 1 => false
    /// -> Filter 2 => true
    /// _____________________
    /// Group 1 => true
    /// Group 2
    /// -> Filter 1 => false
    /// -> Filter 2 => false
    /// _____________________
    /// Group 2 => false
    /// _____________________
    /// Final result: false
    /// </summary>
    internal enum FilterGroups : uint
    {
        Default,
        AddressFilters,
        NotLocalAddress
    }
}
