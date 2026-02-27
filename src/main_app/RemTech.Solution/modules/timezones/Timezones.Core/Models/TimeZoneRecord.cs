namespace Timezones.Core.Models;

public sealed class TimeZoneRecord
{
    public required string ZoneName { get; init; }
    public required ulong GmtOffset { get; init; }
    public required ulong Timestamp { get; init; }
}
