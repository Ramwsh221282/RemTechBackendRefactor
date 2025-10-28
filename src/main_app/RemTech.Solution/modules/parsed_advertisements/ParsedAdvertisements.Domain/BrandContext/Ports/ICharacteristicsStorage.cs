using ParsedAdvertisements.Domain.CharacteristicContext;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.BrandContext.Ports;

public interface ICharacteristicsStorage
{
    Task<Status<Characteristic>> Get(string name, CancellationToken ct = default);
    Task Save(Characteristic characteristic, ITransaction txn, CancellationToken ct = default);
}
