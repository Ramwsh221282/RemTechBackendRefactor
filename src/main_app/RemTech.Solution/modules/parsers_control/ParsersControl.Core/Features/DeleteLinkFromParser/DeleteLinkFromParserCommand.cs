using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.DeleteLinkFromParser;

public sealed record DeleteLinkFromParserCommand(Guid ParserId, Guid LinkId) : ICommand;
