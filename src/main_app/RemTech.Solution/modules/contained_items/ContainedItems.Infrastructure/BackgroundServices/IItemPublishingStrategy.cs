using ContainedItems.Domain.Models;

namespace ContainedItems.Infrastructure.BackgroundServices;

public interface IItemPublishingStrategy
{
	Task Publish(ContainedItem item, CancellationToken ct = default);
	Task PublishMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
}
