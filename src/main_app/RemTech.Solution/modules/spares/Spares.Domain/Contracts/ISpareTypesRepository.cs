using Spares.Domain.Types;

namespace Spares.Domain.Contracts;

public interface ISpareTypesRepository
{
	Task<SpareType> SaveOrFindSimilar(SpareType type, CancellationToken ct = default);
	Task<Dictionary<string, SpareType>> SaveOrFindManySimilar(
		IEnumerable<SpareType> types,
		CancellationToken ct = default
	);
}
