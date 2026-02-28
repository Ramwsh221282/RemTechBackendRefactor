using RemTech.SharedKernel.Core.Handlers;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

internal sealed class GetRegionLocalDateTimesQueryHandler : IQueryHandler<GetRegionLocalDateTimesQuery, IReadOnlyList<RegionLocalDateTime>>
{
    public GetRegionLocalDateTimesQueryHandler(IRegionDateTimesRepository repository)
    {
        _repository = repository;
    }

    private readonly IRegionDateTimesRepository _repository;

    public async Task<IReadOnlyList<RegionLocalDateTime>> Handle(GetRegionLocalDateTimesQuery query, CancellationToken ct = default)
    {
        IReadOnlyList<RegionLocalDateTime> dateTimes = await _repository.ProvideDateTimes();
        return dateTimes;        
    }
}