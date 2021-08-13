using System;

namespace NginxLogAnalyzer.Parser
{
    internal class DateTimeVariable : VariableBase<DateTime>
    {
        public DateTimeVariable(string name, Action<DateTime, AccessEntry> setAction) : base(name, setAction)
        { }

        public override bool IsStartOfBlock(char c)
        {
            return char.IsDigit(c);
        }

        protected override DateTime Parse(string value)
        {
            int offset = -1;

            int day = ParseHelper.ParseNumber(value, ref offset);
            ParseHelper.ParseExactChar(value, ref offset, '/');
            int month = ParseHelper.ParseMonth(value, ref offset);
            ParseHelper.ParseExactChar(value, ref offset, '/');
            int year = ParseHelper.ParseNumber(value, ref offset);
            ParseHelper.ParseExactChar(value, ref offset, ':');
            int hour = ParseHelper.ParseNumber(value, ref offset);
            ParseHelper.ParseExactChar(value, ref offset, ':');
            int minute = ParseHelper.ParseNumber(value, ref offset);
            ParseHelper.ParseExactChar(value, ref offset, ':');
            int second = ParseHelper.ParseNumber(value, ref offset);

            try
            {
                return new DateTime(year, month, day, hour, minute, second);
            }
            catch(Exception ex)
            {
                throw new ParseException($"Can not convert '{value}' to a date! year: {year}; month: {month}; day: {day}; hour: {hour}; minute: {minute}; second: {second}", value, 0, ex);
            }
        }
    }
}
