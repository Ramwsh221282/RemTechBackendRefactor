using Identity.Domain.Roles.Ports;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users;

public interface IUser
{
    UserId Id { get; }
    UserLogin Login { get; }
    UserEmail Email { get; }
    HashedUserPassword Password { get; }
    bool EmailConfirmed { get; }
    Task<Status<User>> Register(
        IUsersStorage users,
        IRolesStorage roles,
        IUserRolesStorage userRoles,
        UserLogin login,
        UserEmail email,
        HashedUserPassword password,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct = default
    );
}
