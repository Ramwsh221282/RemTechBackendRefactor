namespace Timezones.Core.Contracts;

public interface ITimeZonesProvider
{
    Task<Dictionary<string, TimeZoneInfo>> FetchTimeZones(CancellationToken ct = default);
}
