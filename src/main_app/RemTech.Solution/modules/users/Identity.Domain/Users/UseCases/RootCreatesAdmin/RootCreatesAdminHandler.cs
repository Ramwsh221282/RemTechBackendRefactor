using Identity.Domain.Roles.Ports;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.RootCreatesAdmin;

public sealed class RootCreatesAdminHandler(
    IUsersStorage users,
    IRolesStorage roles,
    IUserRolesStorage userRoles,
    IPasswordManager passwordManager,
    IIdentityUnitOfWork unitOfWork
) : ICommandHandler<RootCreatesAdminCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        RootCreatesAdminCommand command,
        CancellationToken ct = default
    )
    {
        UserId creatorId = UserId.Create(command.CreatorId);
        UserEmail email = UserEmail.Create(command.NewUserEmail);
        UserLogin login = UserLogin.Create(command.NewUserLogin);

        User? user = await users.Get(creatorId, ct);
        if (user == null)
            return Error.NotFound("Root пользователь не найден.");

        UserPassword password = UserPassword.Create(command.CreatorPassword);
        if (!user.VerifyPassword(password, passwordManager, out Error error))
            return error;

        RootUser root = new RootUser(user);
        HashedUserPassword userPassword = HashedUserPassword.Random(passwordManager);

        return await root.Register(
            users,
            roles,
            userRoles,
            login,
            email,
            userPassword,
            unitOfWork,
            ct
        );
    }
}
