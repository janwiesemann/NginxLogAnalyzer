using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NginxLogAnalyzer.Analyzer;
using NginxLogAnalyzer.Filter;
using NginxLogAnalyzer.Sources;

namespace NginxLogAnalyzer
{
    internal static class Setup
    {
        private static List<T> GetInstancesOfType<T>()
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

        public static List<AccessEntryFilterBase> GetAccesEntryFilers(string[] args)
        {
            List<AccessEntryFilterBase> ret = GetInstancesOfType<AccessEntryFilterBase>();
            ret.Add(new AccessEntryFilterFieldMatchesValue("Address", e => e.RemoteAddress));

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
