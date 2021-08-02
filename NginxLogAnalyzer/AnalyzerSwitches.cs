using System;

namespace NginxLogAnalyzer
{
    [Flags]
    enum AnalyzerSwitches
    {
        None = 0,

        Addresses = 1 << 1,

        Pages = 1 << 2,

        All = -1
    }
}
