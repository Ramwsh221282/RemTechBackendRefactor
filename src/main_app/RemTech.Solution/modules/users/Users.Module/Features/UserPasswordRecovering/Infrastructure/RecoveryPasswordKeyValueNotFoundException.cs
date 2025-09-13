namespace Users.Module.Features.UserPasswordRecovering.Infrastructure;

internal sealed class RecoveryPasswordKeyValueNotFoundException : Exception
{
    public RecoveryPasswordKeyValueNotFoundException()
        : base("Не найден ключ для сброса пароля.") { }
}
