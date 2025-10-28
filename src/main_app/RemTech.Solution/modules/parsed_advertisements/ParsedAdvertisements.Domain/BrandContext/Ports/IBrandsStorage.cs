using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.BrandContext.Ports;

public interface IBrandsStorage
{
    Task<Status<Brand>> Get(string name, ITransaction txn, CancellationToken ct = default);
    Task Save(Brand brand, ITransaction txn, CancellationToken ct = default);
}
