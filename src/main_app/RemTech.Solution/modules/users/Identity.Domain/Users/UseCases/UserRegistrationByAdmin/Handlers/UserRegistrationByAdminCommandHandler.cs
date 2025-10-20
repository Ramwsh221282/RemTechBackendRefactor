using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Tokens;
using Identity.Domain.Tokens.Ports;
using Identity.Domain.UserRoles;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Input;
using Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Output;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Handlers;

public sealed class UserRegistrationByAdminCommandHandler(
    ITokensStorage tokens,
    IUsersStorage users,
    IUserRolesStorage userRoles,
    IRolesStorage roles,
    IPasswordManager passwordManager,
    IIdentityUnitOfWork unitOfWork
) : ICommandHandler<UserRegistrationByAdminCommand, Status<UserRegistrationByAdminResponse>>
{
    public async Task<Status<UserRegistrationByAdminResponse>> Handle(
        UserRegistrationByAdminCommand command,
        CancellationToken ct = default
    )
    {
        Token adminToken = Token.Create(command.AdminToken);
        RoleId roleId = RoleId.Create(command.RoleId);
        UserLogin login = UserLogin.Create(command.Name);
        UserEmail email = UserEmail.Create(command.Email);
        UserToken? verifiedAdminToken = await tokens.Get(adminToken, RoleName.Admin, ct);
        if (verifiedAdminToken is null)
            return Error.Forbidden("Операция только доступна администратору.");

        User? existing = await users.Get(login, ct);
        if (existing != null)
            return Error.Conflict("Логин пользователя уже занят.");

        existing = await users.Get(email, ct);
        if (existing != null)
            return Error.Conflict("Почта пользователя уже занята.");

        Role? role = await roles.Get(roleId, ct);
        if (role is null)
            return Error.NotFound($"Роль не найдена.");

        User toRegister = new User()
        {
            Id = new UserId(),
            Email = email,
            EmailConfirmed = false,
            Login = login,
            Password = new HashedUserPassword(passwordManager),
        };

        await users.Add(toRegister, ct);
        Status saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        UserRole userRole = new UserRole { RoleId = role.Id, UserId = toRegister.Id };

        await userRoles.Add(userRole, ct);
        saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        return new UserRegistrationByAdminResponse(
            toRegister.Id.Id,
            toRegister.Login.Name,
            toRegister.Email.Email,
            userRole.RoleId.Value
        );
    }
}
