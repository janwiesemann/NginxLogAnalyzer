using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Globalization;

namespace NginxLogAnalyzer
{
    class AccessEntry
    {
        public string RemoteAddress { get; set; }
        public string ServerName { get; set; }
        public string RemoteUser { get; set; }
        public DateTime DateTime { get; set; }
        public Request Request { get; set; }
    }

    class Request
    {
        public string Methode { get; set; }
        public string URI { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendMaxLength(10, Methode);
            sb.Append(' ');
            sb.AppendURI(URI);
            sb.Append(' ');
            sb.AppendMaxLength(10, Version);

            return sb.ToString();
        }
    }

    [DebuggerDisplay("{Address} => {AccessEntrys.Count}")]
    class RemoteAddress
    {
        public RemoteAddress(string address)
        {
            Address = address;
            AccessEntrys = new List<AccessEntry>();
        }

        public string Address { get; }
        public List<AccessEntry> AccessEntrys { get; }
    }

    static class Extensions
    {
        public static void AppendMaxLength(this StringBuilder sb, int length, string str)
        {
            if (str == null)
                return;

            if (str.Length > length)
                str = str.Substring(0, length - 3) + "...";

            sb.Append(str);
        }

        public static void AppendURI(this StringBuilder sb, string url) =>             sb.AppendMaxLength(64, url);
    }

    static class ConsoleEx
    {
        public static void WriteURI(string uri)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendURI(uri);

            Console.Write(sb.ToString());
        }
    }

    [Flags]
    enum AnalyzerSwitches
    {
        None = 0,

        Addresses = 1 << 1,

        Pages = 1 << 2,

        All = -1
    }

    abstract class AccessEntryFilterBase
    {
        public string ParameterName { get; }
        public abstract bool HasValue { get; }

        public AccessEntryFilterBase(string paramName)
        {
            ParameterName = paramName;
        }

        public abstract bool Matches(AccessEntry entry);
    }

    abstract class AccessEntryValueFilterBase<T> : AccessEntryFilterBase
    {
        public T Value { get; }
        public override bool HasValue { get; }

        public AccessEntryValueFilterBase(string paramName) : base(paramName)
        {
            string valStr = GetValue();

            if (valStr != null)
            {
                Value = ConvertToValue(valStr);
                HasValue = Value != null;
            }
            else
                HasValue = false;
        }

        protected abstract T ConvertToValue(string valStr);

        public override string ToString()
        {
            return $"{ParameterName}: {Value}";
        }

        private string GetValue()
        {
            string name = "--" + ParameterName;
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].IndexOf(name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (args[i].IndexOf('=') == name.Length)
                        return args[i].Substring(name.Length + 1);
                    else
                        return null;
                }
            }

            return null;
        }
    }

    class AccessEntryFilterFieldMatchesValue : AccessEntryValueFilterBase<string>
    {
        private readonly Func<AccessEntry, object> fieldSelect;

        public AccessEntryFilterFieldMatchesValue(string paramName, Func<AccessEntry, object> fieldSelect) : base(paramName)
        {
            this.fieldSelect = fieldSelect;
        }

        public override bool Matches(AccessEntry entry)
        {
            object obj = fieldSelect(entry);

            return obj.ToString() == Value;
        }

        protected override string ConvertToValue(string valStr) => valStr;
    }

    class AccessEntryFilterAccessTime : AccessEntryValueFilterBase<DateTime?>
    {
        public AccessEntryFilterAccessTime() : base("accessTime")
        { }

        public override bool Matches(AccessEntry entry)
        {
            if (Value == null)
                return false;

            return entry.DateTime >= Value.Value;
        }

        protected override DateTime? ConvertToValue(string valStr)
        {
            if (DateTime.TryParseExact(valStr, "dd.MM.yyyy-hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime date))
                return date;

            return null;
        }
    }

    static class MainClass
    {
        private static AnalyzerSwitches ParseSwitches(string[] args)
        {
            Dictionary<char, AnalyzerSwitches> map = new Dictionary<char, AnalyzerSwitches>
            {
                { 'A', AnalyzerSwitches.All },
                { 'a', AnalyzerSwitches.Addresses },
                { 'p', AnalyzerSwitches.Pages }
            };

            AnalyzerSwitches ret = AnalyzerSwitches.None;
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-") || args[i].StartsWith("--"))
                    continue;

                for (int j = 1; j < args[i].Length; j++)
                {
                    if (!map.TryGetValue(args[i][j], out AnalyzerSwitches flag))
                        continue;

                    ret = ret | flag;
                }
            }

            if (ret == AnalyzerSwitches.None)
                ret = AnalyzerSwitches.All;

            return ret;
        }

        private static string ReadWhile(string line, ref int offset, Func<char, StringBuilder, bool> check)
        {
            StringBuilder sb = new StringBuilder(line.Length - offset);
            while (line.Length > offset)
            {
                char c = line[offset];

                offset++;

                if (!check(c, sb))
                    break;

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static string ParseBlock(string line, ref int offset) => ReadWhile(line, ref offset, (c, sb) => c != ' ' && c != '\t');

        private static DateTime ParseDateTime(string line, ref int offset)
        {
            ReadWhile(line, ref offset, (c, sb) => c != '[');

            string dateString = ReadWhile(line, ref offset, (c, sb) => c != ']');
            DateTime.TryParseExact(dateString, "dd/MMM/yyyy:hh:mm:ss K", null, DateTimeStyles.None, out DateTime res);

            return res;
        }

        private static AccessEntry ParseAccessLine(string line)
        {
            int offset = 0;

            return new AccessEntry
            {
                RemoteAddress = ParseBlock(line, ref offset),
                ServerName = ParseBlock(line, ref offset),
                RemoteUser = ParseBlock(line, ref offset),
                DateTime = ParseDateTime(line, ref offset),
                Request = ParseRequest(line, ref offset)
            };
        }

        private static Request ParseRequest(string line, ref int offset)
        {
            ReadWhile(line, ref offset, (c, sb) => c != '"');

            string requestStr = ReadWhile(line, ref offset, (c, sb) => c != '"');

            int offsetRequest = 0;
            return new Request
            {
                Methode = ParseBlock(requestStr, ref offsetRequest),
                URI = ParseBlock(requestStr, ref offsetRequest),
                Version = ParseBlock(requestStr, ref offsetRequest)
            };
        }

        private static void ReadFile(string file, bool sourceIsDirectory, Dictionary<string, RemoteAddress> addresses, AccessEntryFilterBase[] accessEntryFilters)
        {
            string filename = Path.GetFileName(file);
            if (sourceIsDirectory && filename.IndexOf("access.log", StringComparison.OrdinalIgnoreCase) != 0)
                return;


            using (Stream stream = OpenFile(file))
            {
                StreamReader reader = new StreamReader(stream);

                string line;
                while((line = reader.ReadLine()) != null)
                {
                    AccessEntry entry = ParseAccessLine(line);
         
                    bool skipEntry = false;
                    foreach (AccessEntryFilterBase item in accessEntryFilters)
                    {
                        if (!item.HasValue)
                            continue;

                        if (!item.Matches(entry))
                        {
                            skipEntry = true;
                            break;
                        }
                    }

                    if (skipEntry)
                        continue;

                    if(!addresses.TryGetValue(entry.RemoteAddress, out RemoteAddress addr))
                    {
                        addr = new RemoteAddress(entry.RemoteAddress);
                        addresses.Add(entry.RemoteAddress, addr);
                    }

                    addr.AccessEntrys.Add(entry);
                }
            }
        }

        private static Stream OpenFile(string file)
        {
            if (!File.Exists(file))
                throw new Exception(file + " is not a file!");

            if (file.IndexOf(".gz", StringComparison.OrdinalIgnoreCase) == file.Length - 3)
                return new GZipStream(File.OpenRead(file), CompressionMode.Decompress);

            return File.OpenRead(file);
        }

        private static bool FindPathArg(string[] args, out string path, out bool isDir)
        {
            string thisPath = Process.GetCurrentProcess().MainModule.FileName;

            foreach (var item in args)
            {
                if (item == thisPath)
                    continue;

                if (Directory.Exists(item))
                {
                    path = item;
                    isDir = true;

                    return true;
                }

                if (File.Exists(item))
                {
                    path = item;
                    isDir = false;

                    return true;
                }
            }

            path = null;
            isDir = false;

            return false;
        }

        private static List<TValue> DictionaryToList<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            List<TValue> ret = new List<TValue>();
            foreach (KeyValuePair<TKey, TValue> item in dic)
                ret.Add(item.Value);

            return ret;
        }

        private static string GetDefaultLogDir()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "/usr/local/var/log/nginx/";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "/var/log/nginx/";

            return null;
        }

        private static List<RemoteAddress> ReadInput(string[] args)
        {
            if (!FindPathArg(args, out string path, out bool isDir))
            {
                Console.WriteLine("Path arg missing!");

                path = GetDefaultLogDir();
                if (path == null)
                    return null;

                if (!Directory.Exists(path))
                    return null;

                isDir = true;
                Console.WriteLine("Using default Path: " + path);
            }

            AccessEntryFilterBase[] accessEntryFilters = GetAccesEntryFilers();
            PrintFilters(accessEntryFilters);

            Dictionary<string, RemoteAddress> addresses = new Dictionary<string, RemoteAddress>();
            if (isDir)
            {
                foreach (var item in Directory.EnumerateFiles(path))
                    ReadFile(item, true, addresses, accessEntryFilters);
            }
            else
                ReadFile(path, false, addresses, accessEntryFilters);

            return DictionaryToList(addresses);
        }

        private static void PrintFilters(AccessEntryFilterBase[] accessEntryFilters)
        {
            WriteHeader("Filters");

            for (int i = 0; i < accessEntryFilters.Length; i++)
            {
                if(accessEntryFilters[i].HasValue)
                    Console.WriteLine(accessEntryFilters[i].ToString());
            }
        }

        private static AccessEntryFilterBase[] GetAccesEntryFilers()
        {
            return new AccessEntryFilterBase[]
            {
                new AccessEntryFilterFieldMatchesValue("Address", e => e.RemoteAddress),
                new AccessEntryFilterAccessTime()
            };
        }

        private static void PrintSwitches(AnalyzerSwitches switches)
        {
            WriteHeader("Using Switches");

            foreach (AnalyzerSwitches item in Enum.GetValues(typeof(AnalyzerSwitches)))
            {
                if (item == AnalyzerSwitches.All || item == AnalyzerSwitches.None)
                    continue;

                if (switches.HasFlag(item))
                    Console.WriteLine(item.ToString());
            }
        }

        public static void Main(string[] args)
        {
            List<RemoteAddress> addresses = ReadInput(args);
            if (addresses == null)
                return;

            AnalyzerSwitches switches = ParseSwitches(args);
            PrintSwitches(switches);

            try
            {
                Analyze(addresses, switches);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static List<List<AccessEntry>> GroupRequests(List<AccessEntry> entries)
        {
            Dictionary<string, List<AccessEntry>> groupes = new Dictionary<string, List<AccessEntry>>();
            foreach (var item in entries)
            {
                string uri = item.Request?.URI ?? "";

                if(!groupes.TryGetValue(uri, out List<AccessEntry> list))
                {
                    list = new List<AccessEntry>();
                    groupes.Add(uri, list);
                }

                list.Add(item);
            }

            return DictionaryToList(groupes);
        }

        private static void PrintMostRequestsByAddress(List<RemoteAddress> addresses)
        {
            WriteHeader("Most Requests by Addresse");

            int i = 0;
            foreach (RemoteAddress address in addresses.OrderByDescending(e => e.AccessEntrys.Count))
            {
                Console.WriteLine(address.Address.PadRight(16) + " => " + address.AccessEntrys.Count);

                int j = 0;
                foreach (var item in GroupRequests(address.AccessEntrys).OrderByDescending(e => e.Count))
                {
                    Console.WriteLine("\t" + item.Count.ToString().PadRight(3) + " => " + item.First().Request);

                    j++;

                    if (j >= 5)
                        break;
                }

                i++;

                if (i >= 10)
                    break;
            }
        }

        private static void WriteHeader(string name)
        {
            Console.WriteLine();

            for (int i = 0; i < Console.WindowWidth-1; i++)
                Console.Write('=');

            Console.WriteLine('=');
            Console.WriteLine(name);
            Console.WriteLine();
        }

        private static void Analyze(List<RemoteAddress> addresses, AnalyzerSwitches switches)
        {
            if(switches.HasFlag( AnalyzerSwitches.Addresses))
                PrintMostRequestsByAddress(addresses);

            if (switches.HasFlag(AnalyzerSwitches.Pages))
                PrintMostRequestsByPage(addresses);
        }

        private static void PrintMostRequestsByPage(List<RemoteAddress> addresses)
        {
            WriteHeader("Most requested Pages");

            Dictionary<string, int> counts = new Dictionary<string, int>();
            int totalCount = 0;
            foreach (var address in addresses)
            {
                foreach (var item in address.AccessEntrys)
                {
                    string uri = item.Request?.URI ?? string.Empty;
                    if (counts.TryGetValue(uri, out int count))
                        counts[uri] = count + 1;
                    else
                        counts.Add(uri, 1);

                    totalCount++;
                }
            }

            if (totalCount == 0)
                return;

            int i = 0;
            foreach (var item in counts.OrderByDescending(i => i.Value))
            {
                int percentage = (int)Math.Round((double)item.Value / totalCount * 100);

                Console.Write(item.Value.ToString().PadRight(4) + " (" +  percentage.ToString().PadLeft(2, '0') + "%) => ");
                ConsoleEx.WriteURI(item.Key);
                Console.WriteLine();

                i++;
                if (i > 25)
                    break;
            }
        }
    }
}
