namespace Cleaners.Module.Domain.Exceptions;

internal sealed class NextRunInvalidException : Exception
{
    public NextRunInvalidException()
        : base("Дата следующего запуска некорректна.") { }
}
