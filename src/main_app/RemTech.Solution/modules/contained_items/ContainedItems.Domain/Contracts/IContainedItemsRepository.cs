using ContainedItems.Domain.Models;

namespace ContainedItems.Domain.Contracts;

public interface IContainedItemsRepository
{
    Task<int> AddMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
}