using Users.Module.CommonAbstractions;

namespace Users.Module.Features.RegisteringUser.Validating;

internal sealed class PasswordLengthValidation(string input, int minLength, Exception onError)
    : IValidation
{
    public void Check()
    {
        if (input.Length < minLength)
            throw onError;
    }
}
