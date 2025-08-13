namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class UserRegistrationRequiresEmailException : Exception
{
    public UserRegistrationRequiresEmailException()
        : base("Для создания учетной записи нужно указать почту") { }

    public UserRegistrationRequiresEmailException(Exception inner)
        : base("Для создания учетной записи нужно указать почту") { }
}
