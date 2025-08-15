namespace Cleaners.Module.Domain.Exceptions;

internal sealed class CleanerHasNoItemsToCleanException : Exception
{
    public CleanerHasNoItemsToCleanException()
        : base("Для чистильщика пока нет записей для очистки.") { }
}
