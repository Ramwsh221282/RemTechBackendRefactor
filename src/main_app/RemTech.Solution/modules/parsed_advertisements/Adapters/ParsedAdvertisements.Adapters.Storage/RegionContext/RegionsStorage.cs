using ParsedAdvertisements.Domain.RegionContext;
using ParsedAdvertisements.Domain.RegionContext.Ports;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Storage.RegionContext;

public sealed class RegionsStorage : IRegionsStorage
{
    public Task<Status<Region>> Get(string name, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Save(Region region, ITransactionManager txn, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}