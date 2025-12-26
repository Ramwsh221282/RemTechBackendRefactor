using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SetWorkTime;

public sealed record SetWorkTimeCommand(Guid Id, long TotalElapsedSeconds) : ICommand;