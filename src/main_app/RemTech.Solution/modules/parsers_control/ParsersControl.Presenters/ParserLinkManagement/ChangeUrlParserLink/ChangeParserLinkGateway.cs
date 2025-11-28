using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUrlChangedListener;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.ChangeUrlParserLink;

public sealed class ChangeParserLinkGateway(
    IEnumerable<IParserLinkUrlChangedListener> listeners,
    FetchParserLinkById fetch
) 
    : IGateway<ChangeParserLinkRequest, ParserLinkResponse>
{
    public async Task<Result<ParserLinkResponse>> Execute(ChangeParserLinkRequest request)
    {
        CancellationToken ct = request.Ct;
        Result<ParserLink> link = await fetch(request.Id, ct);
        if (link.IsFailure) return link.Error;
        PipeLineParserLinkUrlChangedListener pipeLine = new(listeners);
        ParserLink observed = link.Value.AddListener(pipeLine);
        Result<ParserLink> changing = await observed.ChangeUrl(request.NewUrl, ct);
        return changing.IsFailure ? changing.Error : new ParserLinkResponse(changing.Value);
    }
}