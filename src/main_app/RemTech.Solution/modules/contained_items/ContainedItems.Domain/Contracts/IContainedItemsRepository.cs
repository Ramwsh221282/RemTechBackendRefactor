using ContainedItems.Domain.Models;

namespace ContainedItems.Domain.Contracts;

public interface IContainedItemsRepository
{
    public Task<int> AddMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
    public Task UpdateMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
    public Task<ContainedItem[]> Query(ContainedItemsQuery query, CancellationToken ct = default);
}
