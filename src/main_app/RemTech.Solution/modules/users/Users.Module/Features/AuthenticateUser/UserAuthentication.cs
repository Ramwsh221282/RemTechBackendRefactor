using Users.Module.Features.AuthenticateUser.Exceptions;

namespace Users.Module.Features.AuthenticateUser;

internal sealed class UserAuthentication
{
    private string _inputPassword = string.Empty;
    private string _realPassword = string.Empty;

    public void WithRealPassword(string realPassword) => _realPassword = realPassword;

    public void WithInputPassword(string inputPassword) => _inputPassword = inputPassword;

    public bool CanAuth()
    {
        return string.IsNullOrWhiteSpace(_inputPassword) || string.IsNullOrWhiteSpace(_realPassword)
            ? throw new UserPasswordNotProivdedException()
            : BCrypt.Net.BCrypt.Verify(_inputPassword, _realPassword);
    }
}
