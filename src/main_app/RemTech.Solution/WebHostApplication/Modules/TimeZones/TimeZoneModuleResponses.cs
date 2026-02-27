using Timezones.Core.Models;

namespace WebHostApplication.Modules.TimeZones;

public static class TimeZoneModuleResponses
{
    public sealed record TimeZoneRecordResponse(string ZoneName, ulong Offset, ulong TimeStamp, DateTime Date)
    {        
        private const int StartYear = 1970;
        private const int YearsInSeconds = 31_536_000;
        private const uint MonthsInSeconds = 2_628_000;
        private const int DaysInSeconds = 86_400;
        private const int HoursInSeconds = 3_600;               

        public static TimeZoneRecordResponse From(TimeZoneRecord record)
        {            
            DateTime date = CalculateLocalZoneDate(record);
            return new TimeZoneRecordResponse(record.ZoneName, record.GmtOffset, record.Timestamp, date);
        }

        public static IReadOnlyList<TimeZoneRecordResponse> From(IEnumerable<TimeZoneRecord> records)
        {
            return [.. records.Select(From)];
        }

        private static DateTime CalculateLocalZoneDate(TimeZoneRecord record)
        {            
            ulong localSeconds = record.Timestamp;
            int year = CalculateCurrentYear(localSeconds);
            int month = CalculateCurrentMonth(localSeconds);
            int day = CalculateCurrentDay(year, month, localSeconds);
            ulong secondsInCurrentMonth = CalculateSecondsInCurrentMonth(year, month, localSeconds);
            int hour = CalculateCurrentHours(secondsInCurrentMonth);
            int minutes = CalculateCurrentMinutes(secondsInCurrentMonth);
            int seconds = CalculateCurrentSeconds(secondsInCurrentMonth);
            DateTime date = new(year, month, day, hour, minutes, seconds, DateTimeKind.Local);
            return date;
        }

        private static int CalculateCurrentHours(ulong secondsInCurrentMonth)
        {
            ulong secondsInDay = secondsInCurrentMonth % DaysInSeconds;
            return (int)(secondsInDay / HoursInSeconds);
        }

        private static int CalculateCurrentMinutes(ulong secondsInCurrentMonth)
        {
            ulong secondsInDay = secondsInCurrentMonth % DaysInSeconds;
            return (int)(secondsInDay % HoursInSeconds / 60);
        }

        private static int CalculateCurrentSeconds(ulong secondsInCurrentMonth)
        {
            ulong secondsInDay = secondsInCurrentMonth % DaysInSeconds;
            return (int)(secondsInDay % 60);
        }

        private static ulong CalculateSecondsInCurrentMonth(int year, int month, ulong localSeconds)
        {
            int[] daysInMonth = GetDaysInMonthForYear(year);            
            ulong secondsPassed = 0;
            for (int m = 0; m < month - 1; m++)
            {
                secondsPassed += (ulong)daysInMonth[m] * DaysInSeconds;
            }

            return localSeconds - secondsPassed;
        }

        private static int CalculateCurrentDay(int year, int month, ulong localSeconds)
        {
            int[] daysInMonth = GetDaysInMonthForYear(year);            
            ulong secondsPassed = 0;
            for (int m = 0; m < month - 1; m++)
            {
                secondsPassed += (ulong)daysInMonth[m] * DaysInSeconds;
            }

            ulong secondsInCurrentMonth = localSeconds - secondsPassed;
            int currentDay = (int)(secondsInCurrentMonth / DaysInSeconds) + 1;
            if (currentDay < 1)
            {
                currentDay = 1;
            }

            if (currentDay > daysInMonth[month - 1])
            {
                currentDay = daysInMonth[month - 1];
            }

            return currentDay;            
        }

        private static int CalculateCurrentYear(ulong localSeconds)
        {
            return (int)(localSeconds / YearsInSeconds) + StartYear;
        }

        private static int CalculateCurrentMonth(ulong localSeconds)
        {            
            double passedMonths = localSeconds / MonthsInSeconds;
            double currentMonth = passedMonths % 12;
            return (int)Math.Floor(currentMonth);
        }

        private static int[] GetDaysInMonthForYear(int year)
        {
            return DateTime.IsLeapYear(year)
                ? [31,29,31,30,31,30,31,31,30,31,30,31]
                : [31,28,31,30,31,30,31,31,30,31,30,31];
        }
    }
}
