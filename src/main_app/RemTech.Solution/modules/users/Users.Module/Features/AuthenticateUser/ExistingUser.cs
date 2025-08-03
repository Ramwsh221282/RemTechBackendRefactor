using Users.Module.Features.AuthenticateUser.Jwt;

namespace Users.Module.Features.AuthenticateUser;

internal sealed class ExistingUser : IExistingUser
{
    private readonly Guid _id;
    private readonly string _userName;
    private readonly string _email;
    private readonly string _password;
    private readonly bool _emailConfirmed;

    public ExistingUser(
        Guid id,
        string userName,
        string email,
        string password,
        bool emailConfirmed
    )
    {
        _id = id;
        _userName = userName;
        _email = email;
        _password = password;
        _emailConfirmed = emailConfirmed;
    }

    public bool OwnsPassword(UserAuthenticationPassword password)
    {
        UserAuthentication authentication = new();
        password.Print(authentication);
        authentication.WithRealPassword(_password);
        return authentication.CanAuth();
    }

    public UserJsonWebToken PrintToToken(UserJsonWebToken token)
    {
        token.WithUserId(_id);
        return token;
    }
}
