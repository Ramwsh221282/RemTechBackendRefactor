using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.RegionContext.Ports;

public interface IRegionsStorage
{
    Task<Status<Region>> Get(string name, ITransactionManager txn, CancellationToken ct = default);
    Task Save(Region region, ITransactionManager txn, CancellationToken ct = default);
}