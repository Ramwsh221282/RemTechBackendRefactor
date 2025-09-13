namespace Users.Module.Features.UserPasswordRecovering.Exceptions;

internal sealed class UserToRecoverNotFoundException : Exception
{
    public UserToRecoverNotFoundException()
        : base("Учетной записи пользователя с такими данными не существует.") { }
}
