namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanedAmountInvalidException : Exception
{
    public CleanedAmountInvalidException()
        : base("Количество очищенных объявлений было отрицательным.") { }
}
