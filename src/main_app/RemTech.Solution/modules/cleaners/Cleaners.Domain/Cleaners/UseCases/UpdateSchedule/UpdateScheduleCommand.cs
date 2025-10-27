using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.UpdateSchedule;

public sealed record UpdateScheduleCommand(Guid Id, int WaitDays) : ICommand;
