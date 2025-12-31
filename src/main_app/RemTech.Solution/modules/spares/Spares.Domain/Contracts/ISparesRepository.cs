namespace Spares.Domain.Models;

public interface ISparesRepository
{
    Task<int> AddMany(IEnumerable<Spare> spares, CancellationToken ct = default);
}