using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.ChangeWaitDays;

public sealed record ChangeWaitDaysCommand(Guid Id, int WaitDays) : ICommand;
