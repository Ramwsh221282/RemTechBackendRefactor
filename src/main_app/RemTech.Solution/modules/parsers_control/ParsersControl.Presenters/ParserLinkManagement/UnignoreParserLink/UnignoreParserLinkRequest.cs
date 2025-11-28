using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.UnignoreParserLink;

public sealed record UnignoreParserLinkRequest(Guid Id, CancellationToken Ct) : IRequest;