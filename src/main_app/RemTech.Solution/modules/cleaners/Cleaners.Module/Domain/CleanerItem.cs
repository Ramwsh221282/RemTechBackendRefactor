using Cleaners.Module.RabbitMq;

namespace Cleaners.Module.Domain;

internal sealed class CleanerItem(string id, string domain, string sourceUrl)
{
    public void PrintTo(StartCleaningMessage message) => message.Add(id, domain, sourceUrl);
}
