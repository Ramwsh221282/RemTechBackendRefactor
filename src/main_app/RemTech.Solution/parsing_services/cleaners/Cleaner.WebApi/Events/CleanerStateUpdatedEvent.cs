namespace Cleaner.WebApi.Events;

public sealed record CleanerStateUpdatedEvent(Guid Id, long ElapsedSeconds, int ProcessedAmount);
