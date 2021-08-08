using System;
using System.Collections.Generic;

namespace NginxLogAnalyzer.Settings
{
    internal static class Extensions
    {
        public static void ParseValues(this IEnumerable<ISetting> settings, string[] args )
        {
            bool foundValue = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("--"))
                    continue;

                foundValue = true;

                int j = args[i].IndexOf('=');
                if (j < 0)
                {
                    Console.WriteLine($"Param {args[i]} does not have a value!");

                    continue;
                }

                string name = args[i].Substring(2, j - 2);
                string value = args[i].Substring(j + 1);

                foreach (ISetting item in settings)
                {
                    if (name.Length != item.ParameterName.Length || name.IndexOf(item.ParameterName, StringComparison.OrdinalIgnoreCase) != 0)
                        continue;                    

                    item.ParseValue(value);

                    if (!item.HasValue)
                        Console.WriteLine($"Can not use {value} as value for {item.ParameterName}");
                    else
                        Console.WriteLine(item.ToString());

                    break;
                }
            }

            if (!foundValue)
                Console.WriteLine("Found none");
        }

        public static bool TryGetSetting(this IEnumerable<ISetting> settings, string name, out ISetting setting)
        {
            foreach (ISetting item in settings)
            {
                if (item.ParameterName == name)
                {
                    setting = item;

                    return true;
                }
            }

            setting = null;
            return false;
        }

        public static bool TryGetValue<T>(this IEnumerable<ISetting> settings, string name, out T value)
        {
            value = default(T);

            if (!TryGetSetting(settings, name, out ISetting setting))
                return false;

            object val = setting.Value;
            if (val == null || !typeof(T).IsAssignableFrom(val.GetType()))
                return false;

            value = (T)val;

            return true;
        }

        public static void GetCountSettings(this IEnumerable<ISetting> settings, out int addressCount, out int entryCount)
        {
            settings.TryGetValue<int>(Setting.AddressCountSetting, out addressCount);
            settings.TryGetValue<int>(Setting.EntryCountSetting, out entryCount);
        }
    }
}
