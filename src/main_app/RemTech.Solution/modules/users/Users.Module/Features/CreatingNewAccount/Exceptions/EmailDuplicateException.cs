namespace Users.Module.Features.CreatingNewAccount.Exceptions;

internal sealed class EmailDuplicateException : Exception
{
    public EmailDuplicateException()
        : base("Пользователь с таким email уже существует.") { }
}
