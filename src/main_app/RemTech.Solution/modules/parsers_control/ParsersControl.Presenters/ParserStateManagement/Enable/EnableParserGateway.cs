using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement.Defaults;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.Enable;

public sealed class EnableParserGateway(
    IEnumerable<IParserStateChangedEventListener> listeners,
    IParserWorkStatesStorage storage)
    : IGateway<EnableParserGatewayRequest, ParserStateChangeResponse>
{
    private readonly ParserStateChangedEventListenersPipeline _pipeLine = new(listeners);
    
    public async Task<Result<ParserStateChangeResponse>> Execute(EnableParserGatewayRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserWorkTurner? turner = await storage.Fetch(new ParserWorkTurnerQueryArgs(request.Id), ct);
        if (turner == null) return Error.NotFound("Парсер не найден.");
        ParserWorkTurner observed = turner.AddListener(_pipeLine);
        Result<ParserWorkTurner> result = await observed.Enable(ct);
        return result.IsFailure ? result.Error : new ParserStateChangeResponse(result.Value);
    }
}