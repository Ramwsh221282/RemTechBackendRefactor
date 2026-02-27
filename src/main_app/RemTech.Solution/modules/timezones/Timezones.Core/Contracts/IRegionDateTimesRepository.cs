using Timezones.Core.Models;

namespace Timezones.Core.Contracts;

public interface IRegionDateTimesRepository
{
    Task Refresh(IReadOnlyList<RegionLocalDateTime> dateTimes);
    Task<IReadOnlyList<RegionLocalDateTime>> ProvideDateTimes();
}