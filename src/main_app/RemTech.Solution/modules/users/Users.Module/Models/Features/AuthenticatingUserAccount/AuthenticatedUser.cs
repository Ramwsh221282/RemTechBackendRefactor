namespace Users.Module.Models.Features.AuthenticatingUserAccount;

internal sealed class AuthenticatedUser
{
    private readonly Guid _id;
    private readonly string _name;
    private readonly string _email;
    private readonly string _password;
    private readonly bool _emailConfirmed;
    private readonly string _roleName;

    public AuthenticatedUser(
        Guid id,
        string name,
        string email,
        string password,
        bool emailConfirmed,
        string roleName
    )
    {
        _id = id;
        _name = name;
        _email = email;
        _password = password;
        _emailConfirmed = emailConfirmed;
        _roleName = roleName;
    }

    public UserJwtSource AsJwtSource()
    {
        return new UserJwtSource(_id, _name, _email, _password, _roleName);
    }
}
