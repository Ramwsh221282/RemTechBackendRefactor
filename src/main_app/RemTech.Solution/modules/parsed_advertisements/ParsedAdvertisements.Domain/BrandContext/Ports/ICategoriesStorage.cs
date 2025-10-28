using ParsedAdvertisements.Domain.CategoryContext;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.BrandContext.Ports;

public interface ICategoriesStorage
{
    Task<Status<Category>> Get(string name, CancellationToken ct = default);
    Task Save(Category category, ITransaction txn, CancellationToken ct = default);
}
