namespace Cleaners.Module.RabbitMq;

internal sealed record StartCleaningMessage(List<StartCleaningItemInfo> Items)
{
    public void Add(string id, string domain, string sourceUrl)
    {
        Items.Add(new StartCleaningItemInfo(id, domain, sourceUrl));
    }
}
