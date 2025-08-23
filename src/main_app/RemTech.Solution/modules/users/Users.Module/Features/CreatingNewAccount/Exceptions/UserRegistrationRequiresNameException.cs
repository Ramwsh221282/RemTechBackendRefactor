namespace Users.Module.Features.CreatingNewAccount.Exceptions;

internal sealed class UserRegistrationRequiresNameException : Exception
{
    public UserRegistrationRequiresNameException()
        : base("Для создания учетной записи нужно указать название учетной записи.") { }

    public UserRegistrationRequiresNameException(Exception inner)
        : base("Для создания учетной записи нужно указать название учетной записи.", inner) { }
}
