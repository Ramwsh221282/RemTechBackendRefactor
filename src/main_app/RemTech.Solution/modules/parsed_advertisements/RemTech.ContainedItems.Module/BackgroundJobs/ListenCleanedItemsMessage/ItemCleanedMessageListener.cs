using System.Threading.Channels;
using Cleaners.Module.Contracts.ItemCleaned;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RemTech.ContainedItems.Module.Features.RemoveContainedItem;

namespace RemTech.ContainedItems.Module.BackgroundJobs.ListenCleanedItemsMessage;

internal sealed class ItemCleanedMessageListener(
    Channel<ItemCleanedMessage> channel,
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource
) : BackgroundService
{
    private const string Entrance = nameof(ItemCleanedMessageListener);
    private readonly ChannelReader<ItemCleanedMessage> _reader = channel.Reader;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Entrance} started.", Entrance);
        while (!stoppingToken.IsCancellationRequested)
        {
            await _reader.WaitToReadAsync(stoppingToken);
            if (!_reader.TryRead(out ItemCleanedMessage? message))
                continue;
            try
            {
                RemoveContainedItemCommand command = new(message);
                RemoveContainedItemHandler handler = new(logger, dataSource);
                await handler.Handle(command, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.Fatal("{Entrance}. {Ex}.", Entrance, ex.Message);
            }
        }
    }
}
