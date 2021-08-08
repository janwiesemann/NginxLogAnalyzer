using System;

namespace NginxLogAnalyzer.Settings
{
    internal class IntSetting : SettingBase<int>
    {
        public IntSetting(string paramName, int? defaultValue, int? min, int? max) : base(paramName)
        {
            DefaultValue = defaultValue;
            Min = min;
            Max = max;
        }

        public override int Value
        {
            get => base.HasValue ? base.Value : (DefaultValue ?? 0);
        }

        public override bool HasValue
        {
            get => DefaultValue != null || base.HasValue;
        }

        public int? DefaultValue { get; }
        public int? Min { get; }
        public int? Max { get; }

        protected override bool TryParse(string value, out int res)
        {
            if (!int.TryParse(value, out res))
                return false;

            if (Min != null && Min > res)
                return false;

            if (Max != null && Max > res)
                return false;

            return true;
        }
    }
}
