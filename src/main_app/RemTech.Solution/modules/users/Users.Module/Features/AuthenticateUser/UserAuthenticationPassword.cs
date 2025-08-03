namespace Users.Module.Features.AuthenticateUser;

internal sealed class UserAuthenticationPassword(string password)
{
    public void Print(UserAuthentication authentication) =>
        authentication.WithInputPassword(password);
}
