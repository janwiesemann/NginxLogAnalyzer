using System;

namespace NginxLogAnalyzer.Settings
{
    internal class IntSetting : SettingBase<int>
    {
        public IntSetting(string paramName, int? defaultValue, int? min, int? max, string minString = "none", string maxString = "all") : base(paramName)
        {
            DefaultValue = defaultValue;
            Min = min;
            Max = max;
            MinString = minString?.ToUpper();
            MaxString = maxString?.ToUpper();
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
        public string MinString { get; }
        public string MaxString { get; }

        protected override bool TryParse(string value, out int res)
        {
            string upperValue = value?.ToUpper();

            if (MinString != null && MinString == upperValue)
            {
                if (Min != null)
                    res = Min.Value;
                else
                    res = int.MinValue;

                return true;
            }

            if (MaxString != null && MaxString == upperValue)
            {
                if (Max != null)
                    res = Max.Value;
                else
                    res = int.MaxValue;

                return true;
            }

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
