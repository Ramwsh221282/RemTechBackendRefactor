using Users.Module.Models.Features.AuthenticatingUserAccount;
using Users.Module.Models.Features.CreatingNewAccount;
using Users.Module.Models.Features.CreatingNewAccount.Exceptions;

namespace Users.Module.Models;

internal sealed class User
{
    private readonly string _name;
    private readonly string _password;
    private readonly string _email;

    public User(string? name, string password, string? email)
    {
        _name = name ?? string.Empty;
        _email = email ?? string.Empty;
        _password = password;
    }

    public UserRegistration RequireRegistration()
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new UserRegistrationRequiresNameException();
        if (string.IsNullOrWhiteSpace(_email))
            throw new UserRegistrationRequiresEmailException();
        if (string.IsNullOrWhiteSpace(_password))
            throw new UserRegistrationRequiresPasswordException();
        return new UserRegistration(Guid.NewGuid(), _name, _password, _email);
    }

    public IUserAuthentication RequireAuthentication()
    {
        if (string.IsNullOrWhiteSpace(_password))
            throw new AuthenticationPasswordNotProvidedException();
        if (string.IsNullOrWhiteSpace(_email) && !string.IsNullOrWhiteSpace(_name))
            return new NameAuthentication(_name, _password);
        if (string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_email))
            return new EmailAuthentication(_email, _password);
        throw new UnableToResolveAuthenticationMethodException();
    }
}
