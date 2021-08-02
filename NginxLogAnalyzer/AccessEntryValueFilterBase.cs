using System;

namespace NginxLogAnalyzer
{
    abstract class AccessEntryValueFilterBase<T> : AccessEntryFilterBase
    {
        public T Value { get; private set; }

        public AccessEntryValueFilterBase(string paramName) : base(paramName)
        { }

        protected abstract T ConvertToValue(string valStr);

        public override void Parse(string[] args)
        {
            string name = "--" + ParameterName;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].IndexOf(name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (args[i].IndexOf('=') == name.Length)
                    {
                        string val = args[i].Substring(name.Length + 1);
                        Value = ConvertToValue(val);

                        break;
                    }
                }
            }

            HasValue = Value != null;
        }

        public override string ToString()
        {
            return $"{ParameterName}: {Value}";
        }
    }
}
