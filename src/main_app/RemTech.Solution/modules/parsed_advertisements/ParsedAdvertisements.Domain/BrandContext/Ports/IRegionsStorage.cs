using ParsedAdvertisements.Domain.RegionContext;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.BrandContext.Ports;

public interface IRegionsStorage
{
    Task<Status<Region>> Get(string name, CancellationToken ct = default);
    Task Save(Region region, ITransaction txn, CancellationToken ct = default);
}
