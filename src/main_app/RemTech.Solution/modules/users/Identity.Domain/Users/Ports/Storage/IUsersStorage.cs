using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;

namespace Identity.Domain.Users.Ports.Storage;

public interface IUsersStorage
{
    Task<User?> Get(UserEmail email, CancellationToken ct = default);
    Task<User?> Get(UserLogin login, CancellationToken ct = default);
    Task<User?> Get(UserId id, CancellationToken ct = default);
    Task<User?> Get(UserTicketId id, CancellationToken ct = default);
    Task<IEnumerable<User>> Get(RoleName role, CancellationToken ct = default);
    Task<IEnumerable<User>> Get(UsersSpecification specification, CancellationToken ct = default);
}
