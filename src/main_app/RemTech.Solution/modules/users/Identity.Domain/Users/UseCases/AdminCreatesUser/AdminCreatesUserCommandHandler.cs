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

namespace Identity.Domain.Users.UseCases.AdminCreatesUser;

public sealed class AdminCreatesUserCommandHandler(
    IUsersStorage users,
    IRolesStorage roles,
    IPasswordManager passwordManager,
    IValidator<AdminCreatesUserCommand> validator,
    IIdentityUserEventHandler eventsHandler
) : ICommandHandler<AdminCreatesUserCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        AdminCreatesUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        UserId creatorId = UserId.Create(command.CreatorId);
        IdentityUser? creator = await users.Get(creatorId, ct);
        if (creator == null)
            return Error.NotFound("Пользователь не найден.");

        UserPassword creatorPassword = UserPassword.Create(command.CreatorPassword);
        Status verification = creator.Verify(creatorPassword, passwordManager);
        if (verification.IsFailure)
            return verification.Error;

        Role? adminRole = await roles.Get(RoleName.Admin, ct);
        if (adminRole == null)
            return Error.NotFound("Роль не найдена.");

        UserLogin login = UserLogin.Create(command.NewUserLogin);
        UserEmail email = UserEmail.Create(command.NewUserEmail);
        HashedUserPassword newUserPassword = HashedUserPassword.Random(passwordManager);
        IdentityUserProfile profile = new(login, email, newUserPassword);
        IdentityUserSession session = new();
        IdentityUserRoles userRoles = new([adminRole]);
        IdentityUser admin = IdentityUser.Create(profile, session, userRoles);

        Status registration = creator.RegisterUser(admin);
        if (registration.IsFailure)
            return registration.Error;

        Status handling = await creator.PublishEvents(eventsHandler, ct);
        return handling.IsFailure ? handling.Error : admin;
    }
}
