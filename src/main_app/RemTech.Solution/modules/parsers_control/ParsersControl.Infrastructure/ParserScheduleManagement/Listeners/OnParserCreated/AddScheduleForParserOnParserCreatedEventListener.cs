using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnCreated;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnParserCreated;

public sealed class AddScheduleForParserOnParserCreatedEventListener(
    IEnumerable<IParserScheduleCreatedEventListener> listeners
    ) : IOnParserRegisteredEventListener
{
    private readonly ParserScheduleCreatedEventListenerPipeline _pipeLine = new(listeners);
    
    public async Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
    {
        ParserScheduleData scheduleData = new(data.Id, null, null);
        ParserSchedule schedule = new ParserSchedule(scheduleData);
        ParserSchedule observed = schedule.AddListener(_pipeLine);
        Result<ParserSchedule> result = await observed.Register(ct);
        return result.IsFailure ? result.Error : Unit.Value;
    }
}