using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities;
using Identity.Domain.Users.Ports.EventHandlers;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.CreateRoot;

public sealed class CreateRootUserCommandHandler(
    IRolesStorage roles,
    IPasswordManager passwordManager,
    IIdentityUserEventHandler eventsHandler,
    IValidator<CreateRootUserCommand> validator
) : ICommandHandler<CreateRootUserCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        CreateRootUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        Role? role = await roles.Get(RoleName.Root, ct);
        if (role == null)
            return Error.NotFound("Роль не найдена.");

        UserEmail email = UserEmail.Create(command.Email);
        UserLogin login = UserLogin.Create(command.Name);
        UserPassword notHashed = UserPassword.Create(command.Password);
        HashedUserPassword hashed = new HashedUserPassword(notHashed, passwordManager);
        IdentityUserProfile profile = new(login, email, hashed);
        IdentityUserRoles userRoles = new([role]);
        IdentityUser user = IdentityUser.Create(profile, userRoles);

        Status handling = await user.PublishEvents(eventsHandler, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
