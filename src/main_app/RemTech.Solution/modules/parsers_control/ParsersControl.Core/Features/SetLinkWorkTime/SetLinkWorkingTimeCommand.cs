using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetLinkWorkTime;

public sealed record SetLinkWorkingTimeCommand(Guid ParserId, Guid LinkId, long TotalElapsedSeconds) : ICommand;
