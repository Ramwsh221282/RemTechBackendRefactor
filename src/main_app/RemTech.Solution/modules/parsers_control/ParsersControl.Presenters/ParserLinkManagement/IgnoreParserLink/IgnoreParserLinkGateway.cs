using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkIgnoredListener;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.IgnoreParserLink;

public sealed class IgnoreParserLinkGateway(
    IEnumerable<IParserLinkIgnoredListener> listeners,
    FetchParserLinkById fetch
)
    : IGateway<IgnoreParserLinkRequest, ParserLinkResponse>
{
    public async Task<Result<ParserLinkResponse>> Execute(IgnoreParserLinkRequest request)
    {
        CancellationToken ct = request.Ct;
        Result<ParserLink> link = await fetch(request.Id, ct);
        if (link.IsFailure) return link.Error;
        PipeLineParserLinkIgnoredListener pipeLine = new(listeners);
        ParserLink observed = link.Value.AddListener(pipeLine);
        Result<ParserLink> ignored = await observed.MakeIgnored(ct);
        return ignored.IsFailure ? ignored.Error : new  ParserLinkResponse(observed);
    }
}