namespace Users.Module.Features.ChangingEmail.Shared;

internal sealed class PasswordsVerification(string inputPassword, string realPassword)
{
    public bool IsVerified()
    {
        return BCrypt.Net.BCrypt.Verify(inputPassword, realPassword);
    }
}
