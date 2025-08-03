namespace Users.Module.Features.AuthenticateUser.Exceptions;

internal sealed class UserDoesNotExistsException : Exception
{
    public UserDoesNotExistsException()
        : base("Пользователь не найден.") { }

    public UserDoesNotExistsException(Exception inner)
        : base("Пользователь не найден.", inner) { }
}
