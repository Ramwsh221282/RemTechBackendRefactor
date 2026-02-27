namespace Timezones.Core.Models;

public sealed class PlainDateTimeInfo
{
    public required int Year { get; init; }
    public required int Month { get; init; }
    public required int Day { get; init; }
    public required int Hour { get; init; }
    public required int Minutes { get; init; }
    public required int Seconds { get; init; }
}
