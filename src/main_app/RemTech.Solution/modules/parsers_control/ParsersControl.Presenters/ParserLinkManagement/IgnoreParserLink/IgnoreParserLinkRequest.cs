using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.IgnoreParserLink;

public sealed record IgnoreParserLinkRequest(Guid Id, CancellationToken Ct) : IRequest;