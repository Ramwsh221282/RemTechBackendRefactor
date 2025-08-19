using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Features.AddJournalRecord;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Scrapers.Module.Domain.JournalsContext.BackgroundServices.AddJournalRecordListener;

internal sealed class AddJournalRecordBackgroundService(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    ConnectionFactory connectionFactory,
    ActiveScraperJournalsCache cache,
    Serilog.ILogger logger
) : BackgroundService
{
    private const string Exchange = "scrapers";
    private const string Queue = "journals";
    private IConnection? _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    public override async Task StartAsync(CancellationToken ct)
    {
        _connection = await connectionFactory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(
            Exchange,
            ExchangeType.Direct,
            false,
            false,
            cancellationToken: ct
        );
        await _channel.QueueDeclareAsync(Queue, false, false, false, cancellationToken: ct);
        await _channel.QueueBindAsync(Queue, Exchange, Queue, cancellationToken: ct);
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += Handle;
        logger.Information(
            "{Entrance} rabbit mq channel is ready.",
            nameof(AddJournalRecordBackgroundService)
        );
        await base.StartAsync(ct);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (_channel == null)
            throw new ApplicationException(
                $"{nameof(AddJournalRecordBackgroundService)} channel was not initialized."
            );
        if (_consumer == null)
            throw new ApplicationException(
                $"{nameof(AddJournalRecordBackgroundService)} consumer was not initialized."
            );
        await _channel.BasicConsumeAsync(
            Queue,
            consumer: _consumer,
            autoAck: false,
            cancellationToken: ct
        );
        ct.ThrowIfCancellationRequested();
    }

    private async Task Handle(object sender, BasicDeliverEventArgs eventArgs)
    {
        if (_channel == null)
            throw new ApplicationException(
                $"{nameof(AddJournalRecordBackgroundService)} channel was not initialized."
            );
        try
        {
            AddJournalRecordCommand command = AddJournalRecordCommand.FromEventArgs(eventArgs);
            AddJournalRecordHandler handler = new(dataSource, generator, cache);
            await handler.Handle(command);
            logger.Information("Added journal record.");
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Entrance}. {Ex}.",
                nameof(AddJournalRecordBackgroundService),
                ex.Message
            );
        }
        finally
        {
            await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        }
    }
}
