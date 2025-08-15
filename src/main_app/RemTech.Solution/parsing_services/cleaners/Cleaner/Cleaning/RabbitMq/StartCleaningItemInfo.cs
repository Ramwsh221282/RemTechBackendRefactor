namespace Cleaner.Cleaning.RabbitMq;

internal sealed record StartCleaningItemInfo(string Id, string Domain, string SourceUrl);
