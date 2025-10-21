using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities;

public sealed class IdentityUserProfile
{
    public IdentityUserProfile(UserLogin login, UserEmail email, HashedUserPassword password) =>
        (Login, Email, Password) = (login, email, password);

    public IdentityUserProfile(
        UserLogin login,
        UserEmail email,
        HashedUserPassword password,
        bool emailConfirmed = false
    ) => (Login, Email, Password, EmailConfirmed) = (login, email, password, emailConfirmed);

    public UserLogin Login { get; private set; }
    public UserEmail Email { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public HashedUserPassword Password { get; private set; }

    public async Task<Status<IdentityUserProfile>> ChangeEmail(
        IdentityUserProfile profile,
        IUsersStorage users,
        CancellationToken ct = default
    )
    {
        if (!await profile.IsEmailUnique(users, ct))
            return Error.Conflict("Пользователь с такой почтой уже существует.");

        Email = profile.Email;
        return this;
    }

    public void ConfirmEmail() => EmailConfirmed = true;

    public bool HasEmailConfirmed() => EmailConfirmed;

    public bool IsPasswordVerified(
        UserPassword password,
        IPasswordManager manager,
        out Error error
    ) => Password.VerifyPassword(password, manager, out error);

    public async Task<Status<IdentityUserProfile>> ChangeLogin(
        IdentityUserProfile profile,
        IUsersStorage users,
        CancellationToken ct = default
    )
    {
        if (!await profile.IsLoginUnique(users, ct))
            return Error.Conflict("Пользователь с таким логином уже существует.");

        Login = profile.Login;
        return this;
    }

    public async Task<bool> IsLoginUnique(IUsersStorage users, CancellationToken ct = default)
    {
        IdentityUser? user = await users.Get(Login, ct);
        return user == null;
    }

    public async Task<bool> IsEmailUnique(IUsersStorage users, CancellationToken ct = default)
    {
        IdentityUser? user = await users.Get(Email, ct);
        return user == null;
    }

    public void ChangePassword(UserPassword password, IPasswordManager manager) =>
        Password = new HashedUserPassword(password, manager);

    public IdentityUserProfileEventArgs ToEventArgs() =>
        new(Login.Name, Email.Email, Password.Password, HasEmailConfirmed());
}
