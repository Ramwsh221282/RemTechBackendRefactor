namespace Users.Module.Features.UserPasswordRecovering.Core;

internal sealed class UserRecoveringDispatcher
{
    private readonly string? _email;
    private readonly string? _login;

    public UserRecoveringDispatcher(string? email, string? login)
    {
        _email = email;
        _login = login;
    }

    public IUserRecoveringPassword Dispatch()
    {
        if (!string.IsNullOrWhiteSpace(_email))
            return UserRecoveringPasswordByEmail.Create(_email);
        if (!string.IsNullOrWhiteSpace(_login))
            return UserRecoveringPasswordByLogin.Create(_login);
        throw new UnableToDetermineHowToResetPasswordException();
    }
}
