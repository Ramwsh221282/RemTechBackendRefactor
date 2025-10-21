using Identity.Domain.Roles.Ports;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.AdminCreatesUser;

public sealed class AdminCreatesUserCommandHandler(
    IUsersStorage users,
    IUserRolesStorage userRoles,
    IRolesStorage roles,
    IPasswordManager passwordManager,
    IIdentityUnitOfWork unitOfWork
) : ICommandHandler<AdminCreatesUserCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        AdminCreatesUserCommand command,
        CancellationToken ct = default
    )
    {
        UserId creatorId = UserId.Create(command.CreatorId);
        UserPassword creatorPassword = UserPassword.Create(command.CreatorPassword);
        UserEmail newUserEmail = UserEmail.Create(command.NewUserEmail);
        UserLogin newUserLogin = UserLogin.Create(command.NewUserLogin);

        User? creator = await users.Get(creatorId, ct);
        if (creator == null)
            return Error.NotFound("Пользователь не найден.");

        if (!creator.VerifyPassword(creatorPassword, passwordManager, out Error error))
            return error;

        AdminUser admin = new AdminUser(creator);
        HashedUserPassword password = HashedUserPassword.Random(passwordManager);

        return await admin.Register(
            users,
            roles,
            userRoles,
            newUserLogin,
            newUserEmail,
            password,
            unitOfWork,
            ct
        );
    }
}
