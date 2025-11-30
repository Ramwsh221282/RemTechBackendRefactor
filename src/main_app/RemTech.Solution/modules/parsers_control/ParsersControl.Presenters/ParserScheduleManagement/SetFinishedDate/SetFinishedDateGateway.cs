using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnUpdated;
using ParsersControl.Presenters.ParserScheduleManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserScheduleManagement.SetFinishedDate;

public sealed class SetFinishedDateGateway : IGateway<SetFinishedDateRequest, ParserScheduleUpdateResponse>
{
    private readonly IParserScheduleStorage _storage;
    private readonly ParserScheduleUpdatedListenerPipeline _pipeLine;
    
    public async Task<Result<ParserScheduleUpdateResponse>> Execute(SetFinishedDateRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserScheduleQueryArgs args = new(Id: request.Id);
        ParserSchedule? schedule = await _storage.Fetch(args, ct);
        if (schedule == null) return Error.NotFound("Парсер не найден.");
        ParserSchedule observed = schedule.AddListener(_pipeLine);
        Result<ParserSchedule> update = await observed.Update(end: request.FinishedAt, ct: ct);
        return update.IsFailure ? update.Error : new ParserScheduleUpdateResponse(update.Value);
    }
    
    public SetFinishedDateGateway(
        IParserScheduleStorage storage,
        IEnumerable<IParserScheduleUpdatedEventListener> listeners
    )
    {
        _storage = storage;
        _pipeLine = new ParserScheduleUpdatedListenerPipeline(listeners);
    }
}