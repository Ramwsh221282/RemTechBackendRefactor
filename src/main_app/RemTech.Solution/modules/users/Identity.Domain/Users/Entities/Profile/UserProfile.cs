using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Events;

namespace Identity.Domain.Users.Entities.Profile;

public sealed class UserProfile
{
    public UserProfile(UserLogin login, UserEmail email, HashedUserPassword password) =>
        (Login, Email, Password) = (login, email, password);

    public UserProfile(
        UserLogin login,
        UserEmail email,
        HashedUserPassword password,
        bool emailConfirmed = false
    ) => (Login, Email, Password, EmailConfirmed) = (login, email, password, emailConfirmed);

    public UserLogin Login { get; private set; }
    public UserEmail Email { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public HashedUserPassword Password { get; private set; }

    public bool HasEmailConfirmed() => EmailConfirmed;

    public UserProfile Confirmed()
    {
        return new UserProfile(this, true);
    }

    public UserProfile Change(
        UserLogin? login = null,
        UserEmail? email = null,
        HashedUserPassword? password = null
    )
    {
        UserProfile updated = login == null ? this : Change(login);
        updated = email == null ? updated : Change(email);
        updated = password == null ? updated : Change(password);
        return updated;
    }

    public UserProfile Change(HashedUserPassword password)
    {
        return new UserProfile(this, password);
    }

    public UserProfile Change(UserEmail email)
    {
        return new UserProfile(this, email);
    }

    public UserProfile Change(UserLogin login)
    {
        return new UserProfile(this, login);
    }

    public UserProfileEventArgs ToEventArgs() =>
        new(Login.Name, Email.Email, Password.Password, HasEmailConfirmed());

    public UserProfile(UserProfile profile, bool confirmed)
        : this(profile.Login, profile.Email, profile.Password, confirmed) { }

    private UserProfile(UserProfile profile, HashedUserPassword password)
        : this(profile)
    {
        Password = password;
    }

    private UserProfile(UserProfile profile, UserEmail email)
        : this(profile)
    {
        Email = email;
    }

    private UserProfile(UserProfile profile, UserLogin login)
        : this(profile)
    {
        Login = login;
    }

    private UserProfile(UserProfile profile)
    {
        Login = profile.Login;
        Email = profile.Email;
        EmailConfirmed = profile.EmailConfirmed;
        Password = profile.Password;
    }
}
