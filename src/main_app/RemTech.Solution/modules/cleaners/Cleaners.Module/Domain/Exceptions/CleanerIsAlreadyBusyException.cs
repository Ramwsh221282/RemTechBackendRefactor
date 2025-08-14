namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerIsAlreadyBusyException : Exception
{
    public CleanerIsAlreadyBusyException()
        : base("Чистильщик уже работает.") { }
}
