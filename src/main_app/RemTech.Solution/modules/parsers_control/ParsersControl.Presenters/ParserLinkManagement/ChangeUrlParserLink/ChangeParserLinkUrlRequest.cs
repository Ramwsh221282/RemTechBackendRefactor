using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.ChangeUrlParserLink;

public sealed record ChangeParserLinkUrlRequest(Guid Id, string NewUrl, CancellationToken Ct) : IRequest;