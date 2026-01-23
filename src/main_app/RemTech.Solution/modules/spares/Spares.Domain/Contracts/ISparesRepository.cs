using Spares.Domain.Models;

namespace Spares.Domain.Contracts;

public interface ISparesRepository
{
	Task<int> AddMany(IEnumerable<Spare> spares, CancellationToken ct = default);
}
