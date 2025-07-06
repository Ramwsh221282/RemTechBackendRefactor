using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Database;

public interface ITransactional : IDisposable, IAsyncDisposable
{
    Task<Status> Save(CancellationToken ct = default);
}
