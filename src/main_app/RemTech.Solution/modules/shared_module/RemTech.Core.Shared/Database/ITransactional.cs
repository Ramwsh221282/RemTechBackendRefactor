using RemTech.Result.Library;

namespace RemTech.Core.Shared.Database;

public interface ITransactional : IDisposable, IAsyncDisposable
{
    Task<Status> Save(CancellationToken ct = default);
}
