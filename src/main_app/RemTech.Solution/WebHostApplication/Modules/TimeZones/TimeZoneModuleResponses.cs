using Timezones.Core.Models;

namespace WebHostApplication.Modules.TimeZones;

public static class TimeZoneModuleResponses
{
    public sealed record TimeZoneRecordResponse(string ZoneName)
    {
        public static TimeZoneRecordResponse From(TimeZoneRecord record)
        {
            return new TimeZoneRecordResponse(record.ZoneName);
        }

        public static IReadOnlyList<TimeZoneRecordResponse> From(IEnumerable<TimeZoneRecord> records)
        {
            return [.. records.Select(From)];
        }
    }
}
