namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerIsAlreadyWaitingException : Exception
{
    public CleanerIsAlreadyWaitingException()
        : base("Чистильщик уже ожидает.") { }
}
