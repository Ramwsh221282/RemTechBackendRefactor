using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Tokens;
using Identity.Domain.Tokens.Ports;
using Identity.Domain.UserRoles;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.UseCases.UserRegistration.Input;
using Identity.Domain.Users.UseCases.UserRegistration.Output;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration.Handlers;

public sealed class UserRegistrationCommandHandler(
    IPasswordManager passwordManager,
    ITokensStorage tokensStorage,
    IRolesStorage roles,
    IUsersStorage users,
    IUserRolesStorage userRoles,
    IIdentityUnitOfWork unitOfWork
) : ICommandHandler<UserRegistrationCommand, Status<UserRegistrationResponse>>
{
    public async Task<Status<UserRegistrationResponse>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        UserLogin login = UserLogin.Create(command.UserLogin);
        UserEmail email = UserEmail.Create(command.UserEmail);
        UserPassword password = UserPassword.Create(command.UserPassword);
        Status hasConflict = await ConflictCheck(login, email, ct);
        if (hasConflict.IsFailure)
            return hasConflict.Error;

        Status<User> user = await UserCreating(login, email, password, ct);
        if (user.IsFailure)
            return user.Error;

        Status roleAttachment = await UserRoleAttachment(user, ct);
        if (roleAttachment.IsFailure)
            return roleAttachment.Error;

        UserToken token = await tokensStorage.CreateToken(user, ct);
        return new UserRegistrationResponse(token);
    }

    private async Task<Status> ConflictCheck(UserLogin login, UserEmail email, CancellationToken ct)
    {
        User? withLogin = await users.Get(login, ct);
        if (withLogin != null)
            return Status.Failure(Error.Conflict("Логин уже занят."));

        User? withEmail = await users.Get(email, ct);
        if (withEmail != null)
            return Status.Failure(Error.Conflict("Почта уже занята."));

        return Status.Success();
    }

    private async Task<Status<User>> UserCreating(
        UserLogin login,
        UserEmail email,
        UserPassword password,
        CancellationToken ct
    )
    {
        User user = new User
        {
            Password = new HashedUserPassword(password, passwordManager),
            Id = new UserId(),
            Login = login,
            Email = email,
            EmailConfirmed = false,
        };

        await users.Add(user, ct);
        Status saving = await unitOfWork.Save(ct);
        return saving.IsFailure ? saving.Error : user;
    }

    private async Task<Status> UserRoleAttachment(User user, CancellationToken ct)
    {
        RoleName required = RoleName.User;
        Role? role = await roles.Get(required);
        if (role == null)
            return Status.Failure(Error.NotFound($"Не найдена роль: {required.Value}"));

        UserRole userRole = new UserRole { UserId = user.Id, RoleId = role.Id };
        await userRoles.Add(userRole, ct);
        Status saving = await unitOfWork.Save(ct);
        return saving;
    }
}
