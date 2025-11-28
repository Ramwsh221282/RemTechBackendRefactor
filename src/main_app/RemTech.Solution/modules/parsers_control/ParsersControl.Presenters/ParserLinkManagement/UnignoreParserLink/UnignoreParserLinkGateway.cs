using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUnignoredListener;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.UnignoreParserLink;

public sealed class UnignoreParserLinkGateway (
    IEnumerable<IParserLinkUnignoredListener> listeners,
    FetchParserLinkById fetch)
    : IGateway<UnignoreParserLinkRequest, ParserLinkResponse>
{
    public async Task<Result<ParserLinkResponse>> Execute(UnignoreParserLinkRequest request)
    {
        CancellationToken ct = request.Ct;
        ParserLinkQueryArgs query = new(Id: request.Id);
        Result<ParserLink> fetching = await fetch(request.Id, ct);
        if (fetching.IsFailure) return fetching.Error;
        PipeLineParserLinkUnignoredListener pipeline = new(listeners);
        ParserLink observed = fetching.Value.AddListener(pipeline);
        Result<ParserLink> result = await observed.Unignore(ct);
        return result.IsFailure ? result.Error : new ParserLinkResponse(result);
    }
}