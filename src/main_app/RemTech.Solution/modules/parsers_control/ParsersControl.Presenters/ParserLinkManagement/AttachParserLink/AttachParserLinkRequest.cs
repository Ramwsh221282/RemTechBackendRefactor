using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.AttachParserLink;

public sealed record AttachParserLinkRequest(
    string Name, 
    string Url, 
    Guid ParserId, 
    CancellationToken Ct) : IRequest;