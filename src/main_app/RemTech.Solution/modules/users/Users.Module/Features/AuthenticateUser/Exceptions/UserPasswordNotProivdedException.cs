namespace Users.Module.Features.AuthenticateUser.Exceptions;

internal sealed class UserPasswordNotProivdedException : Exception
{
    public UserPasswordNotProivdedException()
        : base("Пароль пользователя не предоставлен.") { }

    public UserPasswordNotProivdedException(Exception inner)
        : base("Пароль пользователя не предоставлен.", inner) { }
}
