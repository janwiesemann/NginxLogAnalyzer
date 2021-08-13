using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NginxVariableExtractor
{
    class Program
    {
        private static StreamWriter OpenFile(string name)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "../../../..", "NginxLogAnalyzer", name);

            path = Path.GetFullPath(path);

            return new StreamWriter(File.Open(path, FileMode.Create));
        }

        static void Main(string[] args)
        {
            Dictionary<string, object> typeMap = new Dictionary<string, object>
            {
                { "time_local", typeof(DateTime) },
                { "status", typeof(int) },
                { "body_bytes_sent", typeof(int) },
                { "request", "Request" }
            };            

            List<string> vars = GetVars();

            WriteSetup(typeMap, vars);            

            WriteAccessEntryClass(typeMap, vars);
        }

        private static void WriteSetup(Dictionary<string, object> typeMap, List<string> vars)
        {
            Dictionary<string, string> assignmentMap = new Dictionary<string, string>
            {
                { "request", "e.Request = Request.ParseRequest(v)" }
            };

            using (StreamWriter writer = OpenFile("VarSetup.cs"))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using NginxLogAnalyzer.Parser;");
                writer.WriteLine();
                writer.WriteLine("namespace NginxLogAnalyzer");
                writer.WriteLine("{");
                writer.WriteLine("    internal static partial class Setup");
                writer.WriteLine("    {");
                writer.WriteLine("        internal static List<IVariable> GetFormatVariables()");
                writer.WriteLine("        {");
                writer.WriteLine("            List<IVariable> ret = GetInstancesOfType<IVariable>();");
                writer.WriteLine();
                foreach (string item in vars)
                {
                    typeMap.TryGetValue(item, out object o);
                    if (o == null)
                        o = typeof(string);

                    writer.Write("            ret.Add(new ");

                    if (o is Type type && type != typeof(string))
                    {
                        if (type == typeof(int) || type == typeof(long))
                            writer.Write("IntVariable");
                        else if (type == typeof(DateTime))
                            writer.Write("DateTimeVariable");
                    }
                    else
                        writer.Write("StringVariable");

                    writer.Write("(\"");
                    writer.Write(item);
                    writer.Write("\", (v, e) => ");
                    if (!assignmentMap.TryGetValue(item, out string assignment))
                        assignment = "e." + VarNameToPropertyName(item) + " = v";
                    writer.Write(assignment);
                    writer.WriteLine("));");
                }
                writer.WriteLine();
                writer.WriteLine("            return ret;");
                writer.WriteLine("        }");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        private static void WriteAccessEntryClass(Dictionary<string, object> typeMap, List<string> vars)
        {
            using(StreamWriter writer = OpenFile("AccessEntry.cs"))
            {
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("namespace NginxLogAnalyzer");
                writer.WriteLine("{");
                writer.WriteLine("    internal class AccessEntry");
                writer.WriteLine("    {");
                foreach (string item in vars)
                {
                    typeMap.TryGetValue(item, out object o);
                    if (o == null)
                        o = typeof(string);

                    writer.Write("        public ");
                    if (o is Type type)
                        writer.Write(type.Name);
                    else
                        writer.Write(o);
                    writer.Write(" ");
                    writer.Write(VarNameToPropertyName(item));
                    writer.WriteLine(" { get; set; }");
                }
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        private static string VarNameToPropertyName(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            bool nextCharIsUpper = true;

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (c == '_')
                {
                    nextCharIsUpper = true;
                    continue;
                }

                if (nextCharIsUpper)
                {
                    c = char.ToUpper(c);
                    nextCharIsUpper = false;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static List<string> GetVars()
        {
            List<string> ret = new List<string>();

            string source = GetSourceFile();

            foreach (Match item in Regex.Matches(source, @"ngx_string\(\""(\w+)\""\)"))
            {
                string varName = item.Groups[1].Value;

                ret.Add(varName);
            }

            ret.Sort();

            return ret;
        }

        private static string GetSourceFile()
        {
            using (WebClient client = new WebClient())
                return client.DownloadString("https://raw.githubusercontent.com/nginx/nginx/master/src/http/ngx_http_variables.c");
        }
    }
}
