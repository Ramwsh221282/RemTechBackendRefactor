using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.CreateNewParser.Database;
using Scrapers.Module.Features.CreateNewParser.Extensions;
using Scrapers.Module.Features.CreateNewParser.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.CreateNewParser.RabbitMq;

internal sealed class NewParsersEntrance(
    ConnectionFactory connectionFactory,
    ConnectionMultiplexer connectionMultiplexer,
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger
) : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(
            "new_parsers",
            durable: false,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel == null)
            return;
        await _channel.QueuePurgeAsync("new_parsers", cancellationToken);
        await _channel.QueueDeleteAsync("new_parsers", cancellationToken: cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
            throw new ApplicationException("New parsers entrance consuming channel is null.");
        AsyncEventingBasicConsumer consumer = new(_channel);
        consumer.ReceivedAsync += ProcessMessage;
        await _channel.BasicConsumeAsync(
            queue: "new_parsers",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );
        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task ProcessMessage(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel == null)
            throw new ApplicationException("New parsers entrance consuming channel is null.");
        byte[] body = ea.Body.ToArray();
        NewParsersMessage? message = JsonSerializer.Deserialize<NewParsersMessage>(body);
        if (message != null)
        {
            NewParser newParser = NewParser.Create(message.Name, message.Type, message.Domain);
            INewParsersStorage storage = new LoggingNewParsersStorage(
                logger,
                new NpgSqlNewParsersStorage(dataSource)
            );
            ICachedNewParsers cached = new LoggingCachedNewParsers(
                logger,
                new CachedNewParsers(connectionMultiplexer)
            );
            try
            {
                await newParser.Store(storage);
                await newParser.Store(cached);
            }
            catch (Exception ex)
            {
                logger.Error(
                    "Error at {Entrance}. {Error}.",
                    nameof(NewParsersEntrance),
                    ex.Message
                );
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
    }
}

internal sealed record NewParsersMessage(string Name, string Type, string Domain);
