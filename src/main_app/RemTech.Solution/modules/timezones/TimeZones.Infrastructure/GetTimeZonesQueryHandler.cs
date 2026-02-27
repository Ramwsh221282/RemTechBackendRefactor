using RemTech.SharedKernel.Core.Handlers;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

internal sealed class GetTimeZonesQueryHandler : IQueryHandler<GetTimeZonesQuery, IReadOnlyList<TimeZoneRecord>>
{
    public GetTimeZonesQueryHandler(ITimeZonesRepository repository)
    {
        _repository = repository;
    }

    private readonly ITimeZonesRepository _repository;

    public async Task<IReadOnlyList<TimeZoneRecord>> Handle(GetTimeZonesQuery query, CancellationToken ct = default)
    {
        Dictionary<string, TimeZoneRecord> zones = await _repository.ProvideTimeZones();
        return TimeZoneRecord.ToList(zones);        
    }
}