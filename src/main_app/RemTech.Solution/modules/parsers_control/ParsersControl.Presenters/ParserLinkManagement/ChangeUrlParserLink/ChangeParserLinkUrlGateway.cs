using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUrlChangedListener;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.ChangeUrlParserLink;

public sealed class ChangeParserLinkUrlGateway(
    IEnumerable<IParserLinkUrlChangedListener> listeners,
    FetchParserLinkById fetch
) 
    : IGateway<ChangeParserLinkUrlRequest, ParserLinkResponse>
{
    public async Task<Result<ParserLinkResponse>> Execute(ChangeParserLinkUrlRequest urlRequest)
    {
        CancellationToken ct = urlRequest.Ct;
        Result<ParserLink> link = await fetch(urlRequest.Id, ct);
        if (link.IsFailure) return link.Error;
        PipeLineParserLinkUrlChangedListener pipeLine = new(listeners);
        ParserLink observed = link.Value.AddListener(pipeLine);
        Result<ParserLink> changing = await observed.ChangeUrl(urlRequest.NewUrl, ct);
        return changing.IsFailure ? changing.Error : new ParserLinkResponse(changing.Value);
    }
}