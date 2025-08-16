using System.Text;
using System.Text.Json;
using Cleaners.Module.RabbitMq;
using Cleaners.Module.Services.Features.FinishJob;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Cleaners.Module.BackgroundJobs.FinishingListening;

internal sealed class FinishCleanerBackgroundListener(
    ConnectionFactory factory,
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger
) : BackgroundService
{
    private const string Entrance = nameof(FinishCleanerBackgroundListener);
    private const string Exchange = RabbitMqConstants.CleanersExchange;
    private const string Queue = RabbitMqConstants.CleanersFinishQueue;
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
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        try
        {
            long seconds = GetSeconds(ea);
            FinishJobCommand command = new(seconds);
            FinishJobHandler handler = new(logger, connection);
            await handler.Handle(command);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Ex}", nameof(FinishCleanerBackgroundListener), ex.Message);
        }
        finally
        {
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
    }

    private static long GetSeconds(BasicDeliverEventArgs ea)
    {
        ReadOnlyMemory<byte> body = ea.Body;
        string json = Encoding.UTF8.GetString(body.Span);
        using JsonDocument document = JsonDocument.Parse(json);
        long elapsed = document.RootElement.GetProperty("elapsed").GetInt64();
        return elapsed;
    }
}
