namespace Users.Module.Features.AuthenticateUser.Exceptions;

internal sealed class UserIsNotAuthenticatedException : Exception
{
    public UserIsNotAuthenticatedException()
        : base("Ошибка авторизации.") { }

    public UserIsNotAuthenticatedException(Exception inner)
        : base("Ошибка авторизации.", inner) { }
}
