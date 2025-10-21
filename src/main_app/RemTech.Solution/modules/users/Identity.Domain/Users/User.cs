using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.UserRoles;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users;

public sealed class User : IUser
{
    public UserId Id { get; }
    public UserLogin Login { get; }
    public UserEmail Email { get; }
    public HashedUserPassword Password { get; }
    public bool EmailConfirmed { get; }

    public Task<Status<User>> Register(
        IUsersStorage users,
        IRolesStorage roles,
        IUserRolesStorage userRoles,
        UserLogin login,
        UserEmail email,
        HashedUserPassword password,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        string message = "Регистрация пользователей доступна только Root и Admin пользователям.";
        Error error = new Error(message, ErrorCodes.Forbidden);
        Status<User> status = Status<User>.Failure(error);
        return Task.FromResult(status);
    }

    public static async Task<Status<User>> New(
        UserLogin login,
        UserEmail email,
        UserPassword password,
        IUsersStorage users,
        IPasswordManager manager,
        CancellationToken ct = default
    )
    {
        HashedUserPassword hashed = new HashedUserPassword(password, manager);
        return await New(login, email, hashed, users, ct);
    }

    public static async Task<Status<User>> New(
        UserLogin login,
        UserEmail email,
        HashedUserPassword password,
        IUsersStorage users,
        CancellationToken ct = default
    )
    {
        User? withLogin = await users.Get(login, ct);
        if (withLogin != null)
            return Error.Conflict("Логин уже занят.");

        User? withEmail = await users.Get(email, ct);
        if (withEmail != null)
            return Error.Conflict("Почта уже занята.");

        return new User(login, email, password);
    }

    public bool VerifyPassword(UserPassword password, IPasswordManager manager, out Error error) =>
        Password.VerifyPassword(password, manager, out error);

    public async Task<Status> AttachRole(
        IRolesStorage roles,
        IUserRolesStorage userRoles,
        RoleName roleName,
        CancellationToken ct = default
    )
    {
        Role? role = await roles.Get(roleName, ct);
        if (role == null)
            return Status.Failure(Error.NotFound($"Не найдена роль: {roleName.Value}"));

        UserRole userRole = new UserRole(this, role);
        await userRoles.Add(userRole, ct);
        return Status.Success();
    }

    private User(
        UserLogin login,
        UserEmail email,
        HashedUserPassword password,
        bool emailConfirmed = false
    ) =>
        (Id, Login, Email, Password, EmailConfirmed) = (
            new UserId(),
            login,
            email,
            password,
            emailConfirmed
        );
}
