using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scrapers.Module.Features.CreateNewParser.Cache;
using Scrapers.Module.Features.CreateNewParser.Database;
using Scrapers.Module.Features.CreateNewParser.Exceptions;
using Scrapers.Module.Features.CreateNewParser.Extensions;
using Scrapers.Module.Features.CreateNewParser.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.CreateNewParser.RabbitMq;

internal sealed class NewParsersEntrance(
    ConnectionFactory connectionFactory,
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger
) : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.Information("{Service} starting...", nameof(NewParsersEntrance));
        _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(
            "new_parsers",
            durable: false,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );
        await _channel.QueuePurgeAsync("new_parsers", cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.Information("{Service} stopping.", nameof(NewParsersEntrance));
        if (_channel == null)
            throw new ApplicationException($"{nameof(NewParsersEntrance)} Channel was null.");
        if (_connection == null)
            throw new ApplicationException($"{nameof(NewParsersEntrance)} Connection was null.");
        await _channel.QueuePurgeAsync("new_parsers", cancellationToken);
        await _channel.QueueDeleteAsync("new_parsers", cancellationToken: cancellationToken);
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Service} has been starting.", nameof(NewParsersEntrance));
        if (_channel == null)
            throw new ApplicationException($"{nameof(NewParsersEntrance)} Channel was null.");
        if (_connection == null)
            throw new ApplicationException($"{nameof(NewParsersEntrance)} Connection was null.");
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
            throw new ApplicationException($"{nameof(NewParsersEntrance)} Channel was null.");
        if (_connection == null)
            throw new ApplicationException($"{nameof(NewParsersEntrance)} Connection was null.");
        byte[] body = ea.Body.ToArray();
        NewParsersMessage? message = JsonSerializer.Deserialize<NewParsersMessage>(body);
        if (message != null)
        {
            NewParser newParser = NewParser.Create(message.Name, message.Type, message.Domain);
            INewParsersStorage storage = new LoggingNewParsersStorage(
                logger,
                new NpgSqlNewParsersStorage(dataSource)
            );
            try
            {
                await newParser.Store(storage);
            }
            catch (ParserNameAndTypeDuplicateException ex)
            {
                logger.Error(
                    "Error at {Entrance}. {Error}.",
                    nameof(NewParsersEntrance),
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                logger.Fatal(
                    "{Entrance} fatal error: {Ex}.",
                    nameof(NewParsersEntrance),
                    ex.Message
                );
                throw;
            }
            finally
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        }
    }
}

internal sealed record NewParsersMessage(string Name, string Type, string Domain);
