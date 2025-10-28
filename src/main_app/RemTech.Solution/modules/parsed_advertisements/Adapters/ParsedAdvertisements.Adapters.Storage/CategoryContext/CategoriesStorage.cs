using ParsedAdvertisements.Domain.CategoryContext;
using ParsedAdvertisements.Domain.CategoryContext.Ports;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Storage.CategoryContext;

public sealed class CategoriesStorage : ICategoriesStorage
{
    public Task<Status<Category>> Get(string name, ITransactionManager transaction, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Save(Category category, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}