using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.EditLinkUrlInfo;

public sealed record EditLinkUrlInfoCommand(Guid ParserId, Guid LinkId, string? NewName, string? NewUrl) : ICommand;
