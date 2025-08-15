using Cleaner.Cleaning.RabbitMq;

namespace Cleaner.Cleaning.Exceptions;

internal sealed class ItemDoesNotPresentException(StartCleaningItemInfo item) : Exception
{
    private readonly StartCleaningItemInfo _item = item;
}
