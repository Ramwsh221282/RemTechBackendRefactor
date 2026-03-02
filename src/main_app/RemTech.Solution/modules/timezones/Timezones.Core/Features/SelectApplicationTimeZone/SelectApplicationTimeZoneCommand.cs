using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace Timezones.Core.Features.SelectApplicationTimeZone;

public sealed record SelectApplicationTimeZoneCommand(
    string ZoneName    
) : ICommand;

public sealed record SelectApplicationTimeZoneHandler 
: ICommandHandler<SelectApplicationTimeZoneCommand, RegionLocalDateTime>
{
    public SelectApplicationTimeZoneHandler(
        ITimeZonesProvider timeZones,
        IRegionDateTimesRepository regionDateTimes)
    {
        _timeZonesProvider = timeZones;
        _regionDateTimes = regionDateTimes;
    }

    private readonly ITimeZonesProvider _timeZonesProvider;
    private readonly IRegionDateTimesRepository _regionDateTimes;

    public async Task<Result<RegionLocalDateTime>> Execute(
        SelectApplicationTimeZoneCommand command, 
        CancellationToken ct = default)
    {
        TimeZoneRecord? timeZone = await _timeZonesProvider.FetchTimeZone(command.ZoneName, ct);
        if (timeZone is null)
        {
            return Error.NotFound($"Временная зона: {command.ZoneName} не найдена");
        }        

        RegionLocalDateTime dateTime = timeZone.ToRegionLocalDateTime();
        await _regionDateTimes.Save(dateTime, ct);
        return dateTime;
    }
}
