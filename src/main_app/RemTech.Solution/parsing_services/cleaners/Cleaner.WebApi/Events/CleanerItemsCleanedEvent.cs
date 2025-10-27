namespace Cleaner.WebApi.Events;

public sealed record CleanerItemsCleanedEvent(IEnumerable<string> Identifiers);
