using ParsedAdvertisements.Domain.CharacteristicContext.Ports;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Storage.CharacteristicContext;

public sealed class CharacteristicsStorage : ICharacteristicsStorage
{
    public Task<IEnumerable<CharacteristicSimilarity>> GetSimilar(IEnumerable<CharacteristicSimilarity> toMatch,
        ITransactionManager transaction, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}