namespace Users.Module.Features.CreatingNewAccount.Exceptions;

internal sealed class NameExceesLengthException : Exception
{
    public NameExceesLengthException()
        : base("Имя превышает длину 50 символов.") { }
}
