
namespace NginxLogAnalyzer
{
    internal static class Count
    {
        public static bool Continue(ref int count)
        {
            count--;

            return count > 0;
        }
    }
}
