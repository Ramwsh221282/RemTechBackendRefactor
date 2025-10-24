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

namespace Identity.Domain.Users.UseCases.UserRegistration;

public sealed class UserRegistrationCommandHandler(
    IStringHashAlgorithm stringHashAlgorithm,
    IRolesStorage roles,
    IValidator<UserRegistrationCommand> validator,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<UserRegistrationCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        RoleName userRoleName = RoleName.User;
        IdentityRole? role = await roles.Get(RoleName.User, ct);
        if (role == null)
            return Error.NotFound($"Роль {userRoleName.Value} не найдена.");

        UserLogin login = UserLogin.Create(command.UserLogin);
        UserEmail email = UserEmail.Create(command.UserEmail);
        UserPassword notHashed = UserPassword.Create(command.UserPassword);
        HashedUserPassword hashed = new HashedUserPassword(notHashed, stringHashAlgorithm);

        UserProfile profile = new(login, email, hashed);
        UserRolesCollection userRolesCollection = new([role]);
        User user = User.Create(profile, userRolesCollection);

        Status status = await user.PublishEvents(dispatcher, ct);
        return status.IsFailure ? status.Error : user;
    }
}
