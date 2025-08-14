namespace Cleaners.Module.Domain.Exceptions;

internal sealed class LastRunInvalidException : Exception
{
    public LastRunInvalidException()
        : base("Дата последнего запуска некорректна.") { }
}
