namespace Cleaner.Cleaning.RabbitMq;

internal sealed record StartCleaningMessage(List<StartCleaningItemInfo> Items);
