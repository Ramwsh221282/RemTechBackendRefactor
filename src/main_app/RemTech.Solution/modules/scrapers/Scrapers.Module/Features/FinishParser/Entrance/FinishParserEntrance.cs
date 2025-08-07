using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scrapers.Module.Features.CreateNewParser.RabbitMq;
using Scrapers.Module.Features.FinishParser.Database;
using Scrapers.Module.Features.FinishParser.Models;

namespace Scrapers.Module.Features.FinishParser.Entrance;

internal sealed class FinishParserEntrance(
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource,
    ConnectionFactory connectionFactory
) : BackgroundService
{
    private const string Exchange = "scrapers";
    private const string Queue = "finish_scraper";
    private IConnection? _connection;
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Service} starting...", nameof(FinishParserEntrance));
        _connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await _channel.ExchangeDeclareAsync(
            "scrapers",
            ExchangeType.Direct,
            false,
            false,
            null,
            cancellationToken: stoppingToken
        );
        await _channel.QueueDeclareAsync(
            "finish_scraper",
            durable: false,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );
        await _channel.QueueBindAsync(
            "finish_scraper",
            "scrapers",
            "finish_scraper",
            null,
            cancellationToken: stoppingToken
        );
        await base.StartAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Service} has been starting.", nameof(FinishParserEntrance));
        if (_channel == null)
            throw new ApplicationException($"{nameof(FinishParserEntrance)} Channel was null.");
        if (_connection == null)
            throw new ApplicationException($"{nameof(FinishParserEntrance)} Connection was null.");
        AsyncEventingBasicConsumer consumer = new(_channel);
        consumer.ReceivedAsync += ProcessMessage;
        await _channel.BasicConsumeAsync(
            queue: "finish_scraper",
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
        ParserFinishedMessage? message = JsonSerializer.Deserialize<ParserFinishedMessage>(body);
        if (message != null)
        {
            try
            {
                IParserToFinishStorage storage = new NpgSqlFinishedParser(dataSource);
                ParserToFinish parser = await storage.Fetch(message.ParserName, message.ParserType);
                FinishedParser finished = parser.Finish(message.TotalElapsedSeconds);
                await finished.Save(storage);
                logger.Information(
                    "Finish parser: {Name} {Type}.",
                    finished.ParserName,
                    finished.ParserType
                );
            }
            catch (Exception ex)
            {
                logger.Fatal("{Entrance} {Ex}", nameof(FinishParserEntrance), ex.Message);
            }
            finally
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        }
    }
}
