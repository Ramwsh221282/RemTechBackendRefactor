namespace Users.Module.Models.Features.CreatingNewAccount.Exceptions;

internal sealed class InvalidEmailFormatException : Exception
{
    public InvalidEmailFormatException()
        : base("Формат почты некорректен") { }
}
