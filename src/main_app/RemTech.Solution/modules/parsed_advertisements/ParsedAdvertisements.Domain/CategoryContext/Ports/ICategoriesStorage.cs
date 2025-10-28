using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.CategoryContext.Ports;

public interface ICategoriesStorage
{
    Task<Status<Category>> Get(string name, ITransactionManager transaction, CancellationToken ct = default);
    Task Save(Category category, ITransactionManager txn, CancellationToken ct = default);
}