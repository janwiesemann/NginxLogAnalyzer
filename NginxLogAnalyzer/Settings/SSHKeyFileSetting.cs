using System;
using Renci.SshNet;

namespace NginxLogAnalyzer.Settings
{
    internal class SSHKeyFileSetting : SettingBase<PrivateKeyFile>
    {
        public SSHKeyFileSetting() : base("keyFile")
        { }

        protected override string GetValueString()
        {
            return Value?.HostKey?.Name;
        }

        protected override bool TryParse(string file, out PrivateKeyFile res)
        {
            int indexOfStart = 0;
            if (file.Length >= 2 && file[1] == ':') //Windwos drive letter detection
                indexOfStart = 2;

            int i = file.IndexOf(':', indexOfStart);
            string password = null;
            if(i > 0)
            {
                password = file.Substring(i+1);
                file = file.Substring(0, i);
            }

            try
            {
                res = new PrivateKeyFile(file, password);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Can not load private ssh {file} usind password {(password == null ? "No": "Yes")}: {ex.Message} Visit https://github.com/sshnet/SSH.NET for more defailts.");

                res = null;
                return false;
            }
        }
    }
}
