using System;

namespace NginxLogAnalyzer.Settings
{
    internal abstract class SettingBase<T> : ISetting<T>
    {
        public virtual T Value { get; private set; }

        public virtual bool HasValue { get; private set; }

        public SettingBase(string paramName)
        {
            ParameterName = paramName;
        }

        public string ParameterName { get; }

        object ISetting.Value => Value;

        public void ParseValue(string value)
        {
            if (TryParse(value, out T res))
            {
                Value = res;
                HasValue = true;
            }
            else
                HasValue = false;
        }

        protected abstract bool TryParse(string value, out T res);

        protected virtual string GetValueString()
        {
            return Value?.ToString();
        }

        public override string ToString()
        {
            return $"{ParameterName}: {(HasValue ? GetValueString() : "empty")}";
        }
    }
}
