using System.Text;
using System.Text.Json;
using Cleaners.Module.Contracts.ItemCleaned;
using Cleaners.Module.RabbitMq;
using Cleaners.Module.Services.Features.ItemCleaned;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Cleaners.Module.BackgroundJobs.CleanItemsListening;

internal sealed class CleanItemBackgroundListener(
    ConnectionFactory factory,
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger,
    ItemCleanedMessagePublisher publisher
) : BackgroundService
{
    private const string Entrance = nameof(CleanItemBackgroundListener);
    private const string Exchange = RabbitMqConstants.CleanersExchange;
    private const string Queue = RabbitMqConstants.CleanersStartQueue;
    private IConnection? _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    public override async Task StartAsync(CancellationToken ct)
    {
        _connection = await factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(Exchange, ExchangeType.Direct, cancellationToken: ct);
        await _channel.QueueDeclareAsync(Queue, false, false, false, cancellationToken: ct);
        await _channel.QueueBindAsync(Queue, Exchange, Queue, cancellationToken: ct);
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += Handle;
        await base.StartAsync(ct);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (_connection == null)
            throw new ApplicationException($"{Entrance} connection is not initialized.");
        if (_channel == null)
            throw new ApplicationException($"{Entrance} channel is not initialized.");
        if (_consumer == null)
            throw new ApplicationException($"{Entrance} consumer is not initialized.");
        await _channel.BasicConsumeAsync(Queue, autoAck: false, _consumer, cancellationToken: ct);
        ct.ThrowIfCancellationRequested();
    }

    private async Task Handle(object _, BasicDeliverEventArgs ea)
    {
        if (_channel == null)
            throw new ApplicationException($"{Entrance} channel is not initialized.");
        string? itemId = GetItemId(ea);
        if (string.IsNullOrWhiteSpace(itemId))
            return;
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        try
        {
            AddCleanedItemHandler handler = new(connection, logger, publisher);
            await handler.Handle(new AddCleanedItem(itemId));
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}", Entrance, ex.Message);
        }
        finally
        {
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
    }

    private static string? GetItemId(BasicDeliverEventArgs ea)
    {
        ReadOnlyMemory<byte> body = ea.Body;
        string json = Encoding.UTF8.GetString(body.Span);
        using JsonDocument jsonDocument = JsonDocument.Parse(json);
        return jsonDocument.RootElement.GetProperty("id").GetString();
    }
}
