namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerIsBusyException : Exception
{
    public CleanerIsBusyException()
        : base("Чистильщик занят.") { }
}
