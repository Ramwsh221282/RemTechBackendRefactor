using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CreateRoot;

public sealed class CreateRootUserCommandHandler(
    IUsersStorage users,
    IRolesStorage roles,
    IUserRolesStorage userRoles,
    IIdentityUnitOfWork unitOfWork,
    IPasswordManager passwordManager
) : ICommandHandler<CreateRootUserCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        CreateRootUserCommand command,
        CancellationToken ct = default
    )
    {
        UserEmail email = UserEmail.Create(command.Email);
        UserLogin login = UserLogin.Create(command.Email);
        UserPassword password = UserPassword.Create(command.Password);

        Status<User> user = await User.New(login, email, password, users, passwordManager, ct);
        if (user.IsFailure)
            return user.Error;

        Status saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        Status roleAttachment = await user.Value.AttachRole(roles, userRoles, RoleName.Root, ct);
        if (roleAttachment.IsFailure)
            return roleAttachment.Error;

        saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        return user;
    }
}
