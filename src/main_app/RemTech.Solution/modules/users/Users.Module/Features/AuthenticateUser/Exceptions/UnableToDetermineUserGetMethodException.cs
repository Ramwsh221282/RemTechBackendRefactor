namespace Users.Module.Features.AuthenticateUser.Exceptions;

internal sealed class UnableToDetermineUserGetMethodException : Exception
{
    public UnableToDetermineUserGetMethodException()
        : base("Не удается определить способ авторизации пользователя.") { }

    public UnableToDetermineUserGetMethodException(Exception inner)
        : base("Не удается определить способ авторизации пользователя.", inner) { }
}
