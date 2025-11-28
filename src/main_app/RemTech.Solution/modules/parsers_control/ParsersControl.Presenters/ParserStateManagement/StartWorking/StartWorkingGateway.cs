using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.StartWorking;

public sealed class StartWorkingGateway(
    IStatefulParsersStorage storage,
    IEnumerable<IOnStatefulParserStateChangedEventListener> listeners
) 
    : IGateway<StartWorkingRequest, ParserStateChangeResponse>
{
    public async Task<Result<ParserStateChangeResponse>> Execute(StartWorkingRequest request)
    {
        CancellationToken ct = request.Ct;
        IGatewayRequestWithParserFetchById fetcher = request;
        ParserStateChangeResponse response = new();
        IEnumerable<IOnStatefulParserStateChangedEventListener> withResponse = [..listeners, response];
        Result<StatefulParser> parser = await StatefulParser.FromStorage(fetcher.FetchMethod(), storage, ct);
        if (parser.IsFailure) return parser.Error;

        OnStatefulParserStateChangedPipeline pipeline = new(listeners);
        StatefulParser observed = parser.Value.AddListener(pipeline);
        Result<Unit> result = await observed.StartWork(ct);
        return result.IsFailure ? result.Error : response;
    }
}