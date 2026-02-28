namespace Timezones.Core.Models;

public sealed class ApplicationDateTime
{    
    public required string TimeZoneName { get; init; }
    public required DateTime DateTime { get; init; }
    public required ulong GmtOffset_Seconds { get; init; }
    public required ulong TimeStamp_Seconds { get; init; }
    public required int Year { get; init; }
    public required int Month { get; init; }
    public required int Day { get; init; }
    public required int Hour { get; init; }
    public required int Minute { get; init; }
    public required int Second { get; init; }    
}

public static class ApplicationDateTimeConstructionModule
{
    extension(ApplicationDateTime)
    {
        public static ApplicationDateTime Create(RegionLocalDateTime regionLocalDateTime)
        {
            PlainDateTimeInfo plainDateTime = regionLocalDateTime.TimeZoneRecord.ToPlainDateTimeInfo();   
            return new ApplicationDateTime
            {
                TimeZoneName = regionLocalDateTime.TimeZoneRecord.ZoneName,
                DateTime = regionLocalDateTime.LocalDateTime,
                GmtOffset_Seconds = regionLocalDateTime.TimeZoneRecord.GmtOffset,
                TimeStamp_Seconds = regionLocalDateTime.TimeZoneRecord.Timestamp,
                Year = plainDateTime.Year,
                Month = plainDateTime.Month,
                Day = plainDateTime.Day,
                Hour = plainDateTime.Hour,
                Minute = plainDateTime.Minutes,
                Second = plainDateTime.Seconds
            };
        }

        public static ApplicationDateTime Create(
            string timeZoneName,
            DateTime dateTime,
            ulong gmtOffset_Seconds,
            ulong timeStamp_Seconds,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second
        )
        {
            return new ApplicationDateTime
            {
                TimeZoneName = timeZoneName,
                DateTime = dateTime,
                GmtOffset_Seconds = gmtOffset_Seconds,
                TimeStamp_Seconds = timeStamp_Seconds,
                Year = year,
                Month = month,
                Day = day,
                Hour = hour,
                Minute = minute,
                Second = second
            };
        }
    }
}


