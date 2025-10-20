using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Ports.Storage;

public interface IIdentityTransactionScope : IDisposable, IAsyncDisposable
{
    Task<Status> Commit(CancellationToken ct = default);
}
