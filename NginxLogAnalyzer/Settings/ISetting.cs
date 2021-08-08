using System;

namespace NginxLogAnalyzer.Settings
{
    internal interface ISetting
    {
        bool HasValue { get; }
        string ParameterName { get; }
        object Value { get; }
        void ParseValue(string value);
    }

    internal interface ISetting<out T> : ISetting
    {
        new T Value { get; }
    }
}
