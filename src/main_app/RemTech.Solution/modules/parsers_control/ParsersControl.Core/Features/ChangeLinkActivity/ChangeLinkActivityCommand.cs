using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.ChangeLinkActivity;

public sealed record ChangeLinkActivityCommand(Guid ParserId, Guid LinkId, bool IsActive) : ICommand;