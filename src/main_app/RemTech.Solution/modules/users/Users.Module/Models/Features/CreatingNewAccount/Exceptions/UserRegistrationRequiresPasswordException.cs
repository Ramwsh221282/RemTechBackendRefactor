namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class UserRegistrationRequiresPasswordException : Exception
{
    public UserRegistrationRequiresPasswordException()
        : base("Для создания учетной записи нужен пароль.") { }

    public UserRegistrationRequiresPasswordException(Exception inner)
        : base("Для создания учетной записи нужен пароль.", inner) { }
}
