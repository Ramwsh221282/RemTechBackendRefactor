namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerIsNotBusyException : Exception
{
    public CleanerIsNotBusyException()
        : base("Чистильщик не в рабочем состоянии.") { }
}
