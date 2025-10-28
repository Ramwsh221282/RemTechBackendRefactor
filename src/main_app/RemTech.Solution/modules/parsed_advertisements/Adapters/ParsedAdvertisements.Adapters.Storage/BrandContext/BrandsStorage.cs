using ParsedAdvertisements.Domain.BrandContext;
using ParsedAdvertisements.Domain.BrandContext.Ports;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Storage.BrandContext;

public sealed class BrandsStorage : IBrandsStorage
{
    public Task<Status<Brand>> Get(string name, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Save(Brand brand, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}