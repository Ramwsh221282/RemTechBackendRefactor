using ContainedItems.Domain.Models;

namespace ContainedItems.Domain.Contracts;

public interface IContainedItemsRepository
{
    Task<int> AddMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
    Task UpdateMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
    Task<ContainedItem[]> Query(ContainedItemsQuery query, CancellationToken ct = default);
}
