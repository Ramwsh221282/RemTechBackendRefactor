using Users.Module.Features.CreatingNewAccount.Exceptions;

namespace Users.Module.CommonAbstractions;

internal sealed class PasswordValidation(string password)
{
    public void Validate()
    {
        if (password.Length < 8)
            throw new PasswordLengthIsNotSatisfiedException();
        if (!password.Any(char.IsUpper))
            throw new PasswordDoesNotContainUpperCharacterException();
        if (!password.Any(char.IsLower))
            throw new PasswordDoesNotContainLowerCharacterException();
        if (!password.Any(char.IsDigit))
            throw new PasswordShouldContainDigitException();
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            throw new PasswordDoesNotContainSpecialSymbolException();
    }
}
