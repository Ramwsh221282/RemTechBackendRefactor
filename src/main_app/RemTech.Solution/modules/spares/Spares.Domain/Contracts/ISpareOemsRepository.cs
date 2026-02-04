using Spares.Domain.Oems;

namespace Spares.Domain.Contracts;

public interface ISpareOemsRepository
{
	Task<SpareOem> SaveOrFindSimilar(SpareOem oem, CancellationToken ct = default);
	Task<Dictionary<string, SpareOem>> SaveOrFindManySimilar(
		IEnumerable<SpareOem> oems,
		CancellationToken ct = default
	);
}
