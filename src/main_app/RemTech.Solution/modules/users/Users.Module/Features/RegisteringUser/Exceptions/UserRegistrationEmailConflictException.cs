namespace Users.Module.Features.RegisteringUser.Exceptions;

internal sealed class UserRegistrationEmailConflictException : Exception
{
    public UserRegistrationEmailConflictException(string email)
        : base($"Почта пользователя: {email} уже занята.") { }

    public UserRegistrationEmailConflictException(string email, Exception inner)
        : base($"Почта пользователя: {email} уже занята.", inner) { }
}
