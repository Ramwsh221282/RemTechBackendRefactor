namespace Cleaner.WebApi.Events;

public sealed record CleanerWorkFinishedEvent(
    Guid Id,
    long TotalElapsedSeconds,
    long ProcessedItems
);
