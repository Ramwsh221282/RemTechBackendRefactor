namespace Timezones.Core.Models;

public sealed class RegionLocalDateTime
{
    public required TimeZoneRecord TimeZoneRecord { get; init; }
    public required DateTime LocalDateTime { get; init; }
}
