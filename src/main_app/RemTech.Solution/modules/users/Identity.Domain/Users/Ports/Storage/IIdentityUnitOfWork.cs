using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Ports.Storage;

public interface IIdentityUnitOfWork
{
    Task<Status> Save(CancellationToken ct = default);
}
