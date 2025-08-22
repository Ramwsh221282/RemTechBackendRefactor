namespace Users.Module.Features.CreateEmailConfirmation;

internal sealed class UserNotFoundException : Exception
{
    public UserNotFoundException()
        : base("Пользователь не найден.") { }
}
