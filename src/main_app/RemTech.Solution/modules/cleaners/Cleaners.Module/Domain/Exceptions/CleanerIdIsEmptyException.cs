namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerIdIsEmptyException : Exception
{
    public CleanerIdIsEmptyException()
        : base("ID чистильщика был пустым.") { }
}
