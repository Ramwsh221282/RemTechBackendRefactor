using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.StartWaiting;

public sealed class StartWaitingGateway(
    IStatefulParsersStorage storage,
    IEnumerable<IOnStatefulParserStateChangedEventListener> listeners) 
    : IGateway<StartWaitingRequest, ParserStateChangeResponse>
{
    public async Task<Result<ParserStateChangeResponse>> Execute(StartWaitingRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserStateChangeResponse response = new();
        IEnumerable<IOnStatefulParserStateChangedEventListener> withResponse = [..listeners, response];
        IGatewayRequestWithParserFetchById fetcher = request;
        Result<StatefulParser> parser = await StatefulParser.FromStorage(fetcher.FetchMethod(), storage, ct);
        if (parser.IsFailure) return parser.Error;
        
        OnStatefulParserStateChangedPipeline pipeline = new(listeners);
        StatefulParser observed = parser.Value.AddListener(pipeline);
        Result<Unit> result = await observed.StartWaiting(ct);
        return result.IsFailure ? result.Error : response;
    }
}