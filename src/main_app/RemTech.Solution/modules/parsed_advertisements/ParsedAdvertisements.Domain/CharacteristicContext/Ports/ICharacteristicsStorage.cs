using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.CharacteristicContext.Ports;

public interface ICharacteristicsStorage
{
    Task<IEnumerable<CharacteristicSimilarity>> GetSimilar(
        IEnumerable<CharacteristicSimilarity> toMatch,
        ITransactionManager transaction,
        CancellationToken ct = default);
}