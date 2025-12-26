using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.AdjustWorkTime;

public sealed record AdjustWorkTimeCommand(Guid Id, long TotalSeconds) : ICommand;
