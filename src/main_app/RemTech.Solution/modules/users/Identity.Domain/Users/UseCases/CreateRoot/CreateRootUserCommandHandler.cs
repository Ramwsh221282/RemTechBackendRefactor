using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities;
using Identity.Domain.Users.Entities.Profile;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Passwords;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.CreateRoot;

public sealed class CreateRootUserCommandHandler(
    IRolesStorage roles,
    IPasswordManager passwordManager,
    IDomainEventsDispatcher eventsHandler,
    IValidator<CreateRootUserCommand> validator
) : ICommandHandler<CreateRootUserCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        CreateRootUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        IdentityRole? role = await roles.Get(RoleName.Root, ct);
        if (role == null)
            return Error.NotFound("Роль не найдена.");

        UserEmail email = UserEmail.Create(command.Email);
        UserLogin login = UserLogin.Create(command.Name);
        UserPassword notHashed = UserPassword.Create(command.Password);
        HashedUserPassword hashed = new HashedUserPassword(notHashed, passwordManager);
        UserProfile profile = new(login, email, hashed);
        UserRolesCollection userRolesCollection = new([role]);
        User user = User.Create(profile, userRolesCollection);

        Status handling = await user.PublishEvents(eventsHandler, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
