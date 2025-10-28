using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.UpdateWorkStatistics;

public sealed record UpdateWorkStatisticsCommand(Guid Id, long ElapsedSeconds, long ProcessedItems)
    : ICommand;
