using Timezones.Core.Models;

namespace Timezones.Core.Contracts;

public interface ITimeZonesProvider
{
    Task<Dictionary<string, TimeZoneRecord>> FetchTimeZones(CancellationToken ct = default);
    Task<TimeZoneRecord?> FetchTimeZone(string zoneName, CancellationToken ct = default);
}
