namespace RemTech.ContainedItems.Module.Types;

internal interface IContainedItemsCleaner
{
    Guid Id { get; }
    long CleanedAmount { get; }
    DateTime LastRun { get; }
    DateTime NextRun { get; }
    int WaitDays { get; }
    string State { get; }
}

internal sealed record ContainedItemsClear(
    Guid Id,
    long CleanedAmount,
    DateTime LastRun,
    DateTime NextRun,
    int WaitDays,
    string State
) : IContainedItemsCleaner;
