using Users.Module.CommonAbstractions;

namespace Users.Module.Models.Features.CreatingNewAccount;

internal sealed class UserRegistrationJwtDetails
{
    private Guid _id = Guid.NewGuid();
    private string _name = string.Empty;
    private string _password = string.Empty;
    private string _email = string.Empty;
    private string _userRole = string.Empty;

    public void AddId(Guid id) => _id = id;

    public void AddName(string name) => _name = name;

    public void AddPassword(string password) => _password = password;

    public void AddEmail(string email) => _email = email;

    public void AddUserRole(string role) => _userRole = role;

    public UserJwt UserJwt(SecurityKeySource securityKey)
    {
        Validate();
        UserJwtSource source = new UserJwtSource(_id, _name, _email, _password, _userRole);
        return source.Provide(securityKey);
    }

    private void Validate()
    {
        if (_id == Guid.Empty)
            throw new ApplicationException($"Empty user id. {nameof(UserRegistrationJwtDetails)}");
        if (string.IsNullOrWhiteSpace(_name))
            throw new ApplicationException(
                $"Empty user name. {nameof(UserRegistrationJwtDetails)}"
            );
        if (string.IsNullOrWhiteSpace(_email))
            throw new ApplicationException(
                $"Empty user email. {nameof(UserRegistrationJwtDetails)}"
            );
        if (string.IsNullOrWhiteSpace(_password))
            throw new ApplicationException($"Empty password. {nameof(UserRegistrationJwtDetails)}");
        if (string.IsNullOrWhiteSpace(_userRole))
            throw new ApplicationException(
                $"Empty user role. {nameof(UserRegistrationJwtDetails)}"
            );
    }
}
