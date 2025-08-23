namespace Users.Module.Features.ConfirmUserEmail;

internal sealed class UserFromConfirmationNotFoundException : Exception
{
    public UserFromConfirmationNotFoundException()
        : base("Пользователь не найден.") { }
}
