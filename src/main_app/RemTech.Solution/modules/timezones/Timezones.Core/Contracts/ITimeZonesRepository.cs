using Timezones.Core.Models;

namespace Timezones.Core.Contracts;

public interface ITimeZonesRepository
{
    Task Refresh(IReadOnlyList<TimeZoneRecord> records);
    Task<Dictionary<string, TimeZoneRecord>> ProvideTimeZones();
}