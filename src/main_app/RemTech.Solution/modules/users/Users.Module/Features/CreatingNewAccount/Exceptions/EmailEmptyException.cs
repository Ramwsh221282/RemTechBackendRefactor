namespace Users.Module.Features.CreatingNewAccount.Exceptions;

internal sealed class EmailEmptyException : Exception
{
    public EmailEmptyException()
        : base("Строка почты была пустой.") { }
}
