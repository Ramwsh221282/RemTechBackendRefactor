using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities;
using Identity.Domain.Users.Ports.EventHandlers;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.RootCreatesAdmin;

public sealed class RootCreatesAdminHandler(
    IUsersStorage users,
    IRolesStorage roles,
    IPasswordManager passwordManager,
    IValidator<RootCreatesAdminCommand> validator,
    IIdentityUserEventHandler eventHandler
) : ICommandHandler<RootCreatesAdminCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        RootCreatesAdminCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        UserId creatorId = UserId.Create(command.CreatorId);
        UserPassword creatorPassword = UserPassword.Create(command.CreatorPassword);

        IdentityUser? user = await users.Get(creatorId, ct);
        if (user == null)
            return Error.NotFound("Root пользователь не найден.");

        Status verification = user.Verify(creatorPassword, passwordManager);
        if (verification.IsFailure)
            return verification.Error;

        Role? adminRole = await roles.Get(RoleName.Admin, ct);
        if (adminRole == null)
            return Error.NotFound($"Не найдена роль.");

        UserEmail email = UserEmail.Create(command.NewUserEmail);
        UserLogin login = UserLogin.Create(command.NewUserLogin);
        UserPassword notHashed = UserPassword.Create(command.CreatorPassword);
        HashedUserPassword hashed = new HashedUserPassword(notHashed, passwordManager);
        IdentityUserProfile profile = new(login, email, hashed);
        IdentityUserRoles userRoles = new([adminRole]);
        IdentityUser admin = IdentityUser.Create(profile, userRoles);

        Status registration = user.RegisterUser(admin);
        if (registration.IsFailure)
            return registration.Error;

        Status handling = await user.PublishEvents(eventHandler, ct);
        return handling.IsFailure ? handling.Error : admin;
    }
}
