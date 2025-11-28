using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.Enable;

public sealed class EnableParserGateway(
    IEnumerable<IOnStatefulParserStateChangedEventListener> listeners,
    IStatefulParsersStorage parsers)
    : IGateway<EnableParserGatewayRequest, ParserStateChangeResponse>
{
    public async Task<Result<ParserStateChangeResponse>> Execute(EnableParserGatewayRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserStateChangeResponse response = new();
        StatefulParserQueryArgs args = new(Id: request.Id);
        IGatewayRequestWithParserFetchById idRequester = request;
        Result<StatefulParser> parser = await StatefulParser.FromStorage(idRequester.FetchMethod(), parsers, request.Ct);
        if (parser.IsFailure) return parser.Error;

        IEnumerable<IOnStatefulParserStateChangedEventListener> withResponse = [..listeners, response];
        OnStatefulParserStateChangedPipeline pipeline = new(listeners);
        StatefulParser observed = parser.Value.AddListener(pipeline);
        Result<Unit> result = await observed.Enable(ct);

        return result.IsFailure ? result.Error : response;
    }
}