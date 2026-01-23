using ContainedItems.Domain.Models;

namespace ContainedItems.Infrastructure.BackgroundServices;

public interface IItemPublishingStrategy
{
    public Task Publish(ContainedItem item, CancellationToken ct = default);
    public Task PublishMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
}
