using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkParserAttachedListener;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.AttachParserLink;

public sealed class AttachParserLinkGateway(
    IEnumerable<IParserLinkParserAttached> listeners,
    IRegisteredParsersStorage parsers
)
    : IGateway<AttachParserLinkRequest, ParserLinkResponse>
{
    public async Task<Result<ParserLinkResponse>> Execute(AttachParserLinkRequest request)
    {
        RegisteredParser? parser = await parsers.Fetch(new RegisteredParserQuery(Id: request.ParserId), request.Ct);
        if (parser == null) return Error.NotFound("Парсер не найден.");
        PipeLineParserLinkAttachedListener pipeLine = new(listeners);
        ParserLink link = new ParserLink(new ParserLinkData(
            Guid.NewGuid(), 
            request.Name, 
            request.Url, 
            true, 
            Guid.Empty)).AddListener(pipeLine);
        Result<ParserLink> attaching = await link.AttachToParser(parser.Id(), request.Ct);
        return attaching.IsFailure ? attaching.Error : new ParserLinkResponse(attaching.Value);
    }
}