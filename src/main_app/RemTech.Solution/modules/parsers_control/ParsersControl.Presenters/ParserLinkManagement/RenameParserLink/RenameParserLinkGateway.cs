using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkRenamedListener;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.RenameParserLink;

public sealed class RenameParserLinkGateway(
    IEnumerable<IParserLinkRenamedListener> listeners,
    FetchParserLinkById fetch
) 
    : IGateway<RenameParserLinkRequest, ParserLinkResponse>
{
    public async Task<Result<ParserLinkResponse>> Execute(RenameParserLinkRequest request)
    {
        CancellationToken ct = request.Ct;
        Result<ParserLink> link = await fetch(request.Id, ct);
        if (link.IsFailure) return link.Error;
        PipeLineParserLinkRenamedListener pipeLine = new(listeners);
        ParserLink observed = link.Value.AddListener(pipeLine);
        Result<ParserLink> renaming = await observed.Rename(request.NewName, ct);
        return renaming.IsFailure ? renaming.Error : new ParserLinkResponse(renaming.Value);
    }
}