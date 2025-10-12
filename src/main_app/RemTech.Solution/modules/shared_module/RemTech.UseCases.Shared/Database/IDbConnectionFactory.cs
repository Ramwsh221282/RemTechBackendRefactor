using System.Data;

namespace RemTech.UseCases.Shared.Database;

public interface IDbConnectionFactory : IDisposable, IAsyncDisposable
{
    Task<IDbConnection> Provide(CancellationToken ct = default);
}
