using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Ports.Storage;

public interface IUsersStorage
{
    Task<IdentityUser?> Get(UserEmail email, CancellationToken ct = default);
    Task<IdentityUser?> Get(UserLogin login, CancellationToken ct = default);
    Task<IdentityUser?> Get(UserId id, CancellationToken ct = default);
    Task<IdentityUser?> Get(IdentityTokenId id, CancellationToken ct = default);
    Task<IEnumerable<IdentityUser>> Get(RoleName role, CancellationToken ct = default);
}
