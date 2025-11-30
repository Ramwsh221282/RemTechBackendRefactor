using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement.Defaults;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.StartWorking;

public sealed class StartWorkingGateway(
    IParserWorkStatesStorage storage,
    IEnumerable<IParserStateChangedEventListener> listeners
) 
    : IGateway<StartWorkingRequest, ParserStateChangeResponse>
{
    private readonly ParserStateChangedEventListenersPipeline _pipeline = new(listeners);
    
    public async Task<Result<ParserStateChangeResponse>> Execute(StartWorkingRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserWorkTurner? turner = await storage.Fetch(new ParserWorkTurnerQueryArgs(request.Id), ct);
        if (turner == null) return Error.NotFound("Парсер не найден");
        ParserWorkTurner observed = turner.AddListener(_pipeline);
        Result<ParserWorkTurner> result = await observed.StartWork(ct);
        return result.IsFailure ? result.Error : new ParserStateChangeResponse(result.Value);
    }
}