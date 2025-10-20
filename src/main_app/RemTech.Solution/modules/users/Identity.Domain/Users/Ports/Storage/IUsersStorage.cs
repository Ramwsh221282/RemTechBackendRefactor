using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Ports.Storage;

public interface IUsersStorage
{
    Task Add(User user, CancellationToken ct = default);
    Task<User?> Get(UserEmail email, CancellationToken ct = default);
    Task<User?> Get(UserLogin login, CancellationToken ct = default);
}
