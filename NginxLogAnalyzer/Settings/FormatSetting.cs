using System;

namespace NginxLogAnalyzer.Settings
{
    internal class FormatSetting : SettingBase<string>
    {
        public FormatSetting() : base("format")
        { }

        public override bool HasValue => true;

        public override string Value => base.HasValue ? base.Value : "$remote_addr - $remote_user [$time_local] \"$request\" $status $body_bytes_sent \"$http_referer\" \"$http_user_agent\"";

        protected override bool TryParse(string value, out string res)
        {
            res = value;

            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
