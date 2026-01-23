using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.UpdateParserLink;

public sealed record UpdateParserLinkCommand(Guid ParserId, Guid LinkId, string? Name, string? Url) : ICommand;
