using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.UserRoles;
using Identity.Domain.UserRoles.Ports;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users;

public sealed class AdminUser : IUser
{
    private readonly User _user;

    public AdminUser(User user) => _user = user;

    public UserId Id => _user.Id;
    public UserLogin Login => _user.Login;
    public UserEmail Email => _user.Email;
    public HashedUserPassword Password => _user.Password;
    public bool EmailConfirmed => _user.EmailConfirmed;

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

        UserRole? currentRole = await userRoles.Get(_user.Id, RoleName.Admin, ct);
        if (currentRole == null)
            return new Error("Функция доступна только администратору.", ErrorCodes.Forbidden);

        Status<User> created = await User.New(login, email, password, users, ct);

        Status saving = await unitOfWork.Save(ct);
        if (saving.IsFailure)
            return saving.Error;

        Status roleAttachment = await created.Value.AttachRole(roles, userRoles, RoleName.User, ct);
        if (roleAttachment.IsFailure)
            return roleAttachment.Error;

        saving = await unitOfWork.Save(ct);
        return saving.IsFailure ? saving.Error : created;
    }
}
