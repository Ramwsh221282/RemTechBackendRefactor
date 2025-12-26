using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.ChangeSchedule;

public sealed record ChangeScheduleCommand(Guid Id, int WaitDays, DateTime NextRun) : ICommand;