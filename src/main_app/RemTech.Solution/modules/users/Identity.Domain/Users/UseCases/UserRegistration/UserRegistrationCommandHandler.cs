using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistration;

public sealed class UserRegistrationCommandHandler(
    IPasswordManager passwordManager,
    IRolesStorage roles,
    IUsersStorage users,
    IUserRolesStorage userRoles,
    IIdentityUnitOfWork unitOfWork
) : ICommandHandler<UserRegistrationCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        UserRegistrationCommand command,
        CancellationToken ct = default
    )
    {
        UserLogin login = UserLogin.Create(command.UserLogin);
        UserEmail email = UserEmail.Create(command.UserEmail);
        UserPassword password = UserPassword.Create(command.UserPassword);

        // создание пользователя.
        Status<User> user = await User.New(login, email, password, users, passwordManager, ct);
        if (user.IsFailure)
            return user.Error;

        // сохранение изменений.
        Status saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        // прикрепление роли пользователю.
        Status roleAttachment = await user.Value.AttachRole(roles, userRoles, RoleName.User, ct);
        if (roleAttachment.IsFailure)
            return roleAttachment.Error;

        // сохранение изменений.
        saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        return user;
    }
}
