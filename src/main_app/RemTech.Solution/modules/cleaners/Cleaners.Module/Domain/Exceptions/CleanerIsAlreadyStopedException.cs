namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerIsAlreadyStopedException : Exception
{
    public CleanerIsAlreadyStopedException()
        : base("Чистильщик уже отключен.") { }
}
