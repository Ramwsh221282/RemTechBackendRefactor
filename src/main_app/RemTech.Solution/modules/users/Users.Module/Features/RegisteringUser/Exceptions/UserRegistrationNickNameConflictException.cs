namespace Users.Module.Features.RegisteringUser.Exceptions;

internal sealed class UserRegistrationNickNameConflictException : Exception
{
    public UserRegistrationNickNameConflictException(string nickName)
        : base($"Никнейм пользователя {nickName} уже занят.") { }

    public UserRegistrationNickNameConflictException(string nickName, Exception inner)
        : base($"Никнейм пользователя {nickName} уже занят.", inner) { }
}
