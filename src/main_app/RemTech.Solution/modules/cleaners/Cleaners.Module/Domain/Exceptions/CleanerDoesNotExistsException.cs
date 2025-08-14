namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerDoesNotExistsException : Exception
{
    public CleanerDoesNotExistsException()
        : base("Чистильщика нет в системе.") { }
}
