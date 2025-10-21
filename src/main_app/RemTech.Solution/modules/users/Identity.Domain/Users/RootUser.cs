using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.UserRoles;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users;

public sealed class RootUser : IUser
{
    private readonly User _user;

    public RootUser(User user) => _user = user;

    public async Task<Status<User>> Register(
        IUsersStorage users,
        IRolesStorage roles,
        IUserRolesStorage userRoles,
        UserLogin login,
        UserEmail email,
        HashedUserPassword password,
        IIdentityUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        if (!EmailConfirmed)
            return new Error("Функция доступна при подтвержденной почте.", ErrorCodes.Forbidden);

        UserRole? role = await userRoles.Get(_user.Id, RoleName.Root, ct);
        if (role == null)
            return new Error(
                "Создание администраторов доступно только Root пользователю.",
                ErrorCodes.Forbidden
            );

        Status<User> created = await User.New(login, email, password, users, ct);
        if (created.IsFailure)
            return created.Error;

        Status saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        Status roleAttachment = await created.Value.AttachRole(
            roles,
            userRoles,
            RoleName.Admin,
            ct
        );

        if (roleAttachment.IsFailure)
            return roleAttachment.Error;

        saving = await unitOfWork.Save(ct);
        return saving.IsFailure ? saving.Error : created;
    }

    public UserId Id => _user.Id;
    public UserLogin Login => _user.Login;
    public UserEmail Email => _user.Email;
    public HashedUserPassword Password => _user.Password;
    public bool EmailConfirmed => _user.EmailConfirmed;
}
