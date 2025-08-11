using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RemTech.ContainedItems.Module.Features.AddContainedItem;
using RemTech.ContainedItems.Module.Types;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.MessageBus;

internal sealed class AddContainedItemsBus(
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger,
    Channel<AddContainedItemMessage> channel
) : BackgroundService
{
    private readonly ChannelReader<AddContainedItemMessage> _reader = channel.Reader;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.Information("{Entrance} starting.", nameof(AddContainedItemsBus));
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Entrance} started.", nameof(AddContainedItemsBus));
        while (!stoppingToken.IsCancellationRequested)
        {
            await _reader.WaitToReadAsync(stoppingToken);
            while (_reader.TryRead(out AddContainedItemMessage? message))
            {
                IContainedItem item = ContainedItem.New(
                    message.Id,
                    message.Type,
                    message.Domain,
                    message.SourceUrl
                );
                AddContainedItemCommand command = new AddContainedItemCommand(item);
                ICommandHandler<AddContainedItemCommand, int> handler = new AddContainedItemHandler(
                    logger,
                    dataSource
                );
                await handler.Handle(command, stoppingToken);
            }
        }
    }
}
