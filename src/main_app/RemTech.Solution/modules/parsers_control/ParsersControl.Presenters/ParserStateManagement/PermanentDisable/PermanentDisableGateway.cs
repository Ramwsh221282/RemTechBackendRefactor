using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.PermanentDisable;

public sealed class PermanentDisableGateway(
    IEnumerable<IOnStatefulParserStateChangedEventListener> listeners,
    IStatefulParsersStorage storage
)
    : IGateway<PermanentDisableRequest, ParserStateChangeResponse>
{
    public async Task<Result<ParserStateChangeResponse>> Execute(PermanentDisableRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserStateChangeResponse response = new();
        IGatewayRequestWithParserFetchById fetcher = request;
        IEnumerable<IOnStatefulParserStateChangedEventListener> withResponse = [..listeners, response];
        Result<StatefulParser> parser = await StatefulParser.FromStorage(fetcher.FetchMethod(), storage, ct);
        if (parser.IsFailure) return parser.Error;

        OnStatefulParserStateChangedPipeline pipeline = new(withResponse);
        StatefulParser observed = parser.Value.AddListener(pipeline);
        Result<Unit> processing = await observed.PermanentDisable(ct);

        return processing.IsFailure ? processing.Error : response;
    }
}