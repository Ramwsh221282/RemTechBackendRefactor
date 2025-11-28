using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.Disable;

public sealed class DisableParserGateway (
    IStatefulParsersStorage storage,
    IEnumerable<IOnStatefulParserStateChangedEventListener> listeners
    )
    : IGateway<DisableParserRequest, ParserStateChangeResponse>
{
    public async Task<Result<ParserStateChangeResponse>> Execute(DisableParserRequest request)
    {
        ParserStateChangeResponse response = new();
        CancellationToken ct = request.Ct;
        IGatewayRequestWithParserFetchById fetchById = request;
        Result<StatefulParser> parser = await StatefulParser.FromStorage(fetchById.FetchMethod(), storage, ct);
        if (parser.IsFailure) return parser.Error;

        IEnumerable<IOnStatefulParserStateChangedEventListener> withResponse = [..listeners, response];
        OnStatefulParserStateChangedPipeline pipeline = new(listeners);
        StatefulParser observed = parser.Value.AddListener(pipeline);
        Result<Unit> result = await observed.Disable(ct);

        return result.IsFailure ? result.Error : response;
    }
}