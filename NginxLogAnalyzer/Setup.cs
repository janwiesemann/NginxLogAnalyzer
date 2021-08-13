using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NginxLogAnalyzer.Analyzer;
using NginxLogAnalyzer.Filters;
using NginxLogAnalyzer.Parser;
using NginxLogAnalyzer.Settings;
using NginxLogAnalyzer.Sources;

namespace NginxLogAnalyzer
{
    internal static class Setup
    {

        private static List<T> GetInstancesOfType<T>()
        {
            return GetInstancesOfType<T>(null);
        }

        internal static List<IVariable> GetFormatVariables()
        {
            List<IVariable> ret = GetInstancesOfType<IVariable>();
            ret.Add(new StringVariable("remote_addr", (s, e) => e.RemoteAddress = s));
            ret.Add(new StringVariable("remote_user", (s, e) => e.RemoteUser = s));
            ret.Add(new DateTimeVariable("time_local", (dt, e) => e.DateTime = dt));
            ret.Add(new StringVariable("request", (s, e) => e.Request = Request.ParseRequest(s)));
            ret.Add(new IntVariable("status", (i, e) => e.StatusCode = i));
            ret.Add(new StringVariable("body_bytes_sent", (i, e) => { }));
            ret.Add(new StringVariable("http_referer", (i, e) => { }));
            ret.Add(new StringVariable("http_user_agent", (i, e) => { }));

            return ret;
        }

        private static List<T> GetInstancesOfType<T>(Predicate<Type> additionalTypeFilter)
        {
            List<T> ret = new List<T>();
            foreach (Type type in typeof(T).Assembly.GetTypes())
            {
                if (!typeof(T).IsAssignableFrom(type))
                    continue;

                if (type.IsAbstract)
                    continue;

                if (type.IsInterface)
                    continue;

                if (additionalTypeFilter != null && !additionalTypeFilter(type))
                    continue;

                bool hasConstructorWithouParameters = false;
                foreach (ConstructorInfo ctr in type.GetConstructors())
                {
                    if (ctr.GetParameters().Length == 0)
                    {
                        hasConstructorWithouParameters = true;
                        break;
                    }
                }

                if (!hasConstructorWithouParameters)
                    continue;

                T t = (T)Activator.CreateInstance(type);
                ret.Add(t);
            }

            return ret;
        }

        public static List<ISetting> GetSettings(string[] args)
        {
            List<ISetting> ret = GetInstancesOfType<ISetting>(t => !typeof(IFilter).IsAssignableFrom(t));
            ret.Add(new IntSetting(Setting.AddressCountSetting, 25, 1, null));
            ret.Add(new IntSetting(Setting.EntryCountSetting, 5, 1, null));

            return ret;
        }

        public static List<IFilter> GetAccesEntryFilers(string[] args)
        {
            List<IFilter> ret = GetInstancesOfType<IFilter>();
            ret.Add(new FieldValueFilter("Address", e => e.RemoteAddress));

            return ret;
        }

        public static List<ILogSource> GetLogSources(string[] args)
        {
            return GetInstancesOfType<ILogSource>();
        }

        public static string GetDefaultLogDir()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "/usr/local/var/log/nginx/";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "/var/log/nginx/";

            return null;
        }

        internal static List<IAnalyzer> GetAnalyzers(string[] args)
        {
            return GetInstancesOfType<IAnalyzer>();
        }
    }
}
