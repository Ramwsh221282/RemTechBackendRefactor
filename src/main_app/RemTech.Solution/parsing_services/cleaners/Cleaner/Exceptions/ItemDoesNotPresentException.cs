using Cleaner.RabbitMq;
using RabbitMQ.Client;

namespace Cleaner.Exceptions;

internal sealed class ItemDoesNotPresentException(string id) : Exception
{
    public void Log(Serilog.ILogger logger)
    {
        logger.Information("Item with ID {Id} does not present.", id);
    }

    public async Task Publish(IConnection connection)
    {
        DeleteItemEvent @event = new DeleteItemEvent(connection, id);
        await @event.Publish();
    }
}
