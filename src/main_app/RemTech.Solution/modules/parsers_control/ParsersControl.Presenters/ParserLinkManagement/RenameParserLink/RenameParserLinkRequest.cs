using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.RenameParserLink;

public sealed record RenameParserLinkRequest(Guid Id, string NewName, CancellationToken Ct) : IRequest;
