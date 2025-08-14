namespace Cleaners.Module.Domain.Exceptions;

internal sealed class InvalidWaitDaysException : Exception
{
    public InvalidWaitDaysException(int days)
        : base($"Дни ожидания {days} некорректны.") { }
}
