using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement.Defaults;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.StartWaiting;

public sealed class StartWaitingGateway(
    IParserWorkStatesStorage storage,
    IEnumerable<IParserStateChangedEventListener> listeners) 
    : IGateway<StartWaitingRequest, ParserStateChangeResponse>
{
    private readonly ParserStateChangedEventListenersPipeline _pipeline = new(listeners);
    
    public async Task<Result<ParserStateChangeResponse>> Execute(StartWaitingRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserWorkTurner? turner = await storage.Fetch(new ParserWorkTurnerQueryArgs(request.Id), ct);
        if (turner == null) return Error.NotFound("Парсер не найден");
        ParserWorkTurner observed = turner.AddListener(_pipeline);
        Result<ParserWorkTurner> result = await observed.StartWait(ct);
        return result.IsFailure ? result.Error : new ParserStateChangeResponse(result.Value);
    }
}