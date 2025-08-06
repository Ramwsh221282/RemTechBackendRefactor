using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scrapers.Module.Features.FinishParser.Entrance;
using Scrapers.Module.Features.FinishParserLink.Database;
using Scrapers.Module.Features.FinishParserLink.Models;

namespace Scrapers.Module.Features.FinishParserLink.Entrance;

internal sealed class FinishedParserLinkEntrance(
    Serilog.ILogger logger,
    ConnectionFactory connectionFactory,
    NpgsqlDataSource dataSource
) : BackgroundService
{
    private const string Exchange = "scrapers";
    private const string Queue = "finish_scraper_link";
    private IConnection? _connection;
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Service} starting...", nameof(FinishedParserLinkEntrance));
        _connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await _channel.ExchangeDeclareAsync(
            Exchange,
            ExchangeType.Direct,
            false,
            false,
            null,
            cancellationToken: stoppingToken
        );
        await _channel.QueueDeclareAsync(
            Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );
        await _channel.QueueBindAsync(
            Queue,
            Exchange,
            Queue,
            null,
            cancellationToken: stoppingToken
        );
        await base.StartAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("{Service} has been starting.", nameof(FinishedParserLinkEntrance));
        if (_channel == null)
            throw new ApplicationException(
                $"{nameof(FinishedParserLinkEntrance)} Channel was null."
            );
        if (_connection == null)
            throw new ApplicationException(
                $"{nameof(FinishedParserLinkEntrance)} Connection was null."
            );
        AsyncEventingBasicConsumer consumer = new(_channel);
        consumer.ReceivedAsync += ProcessMessage;
        await _channel.BasicConsumeAsync(
            queue: Queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );
        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task ProcessMessage(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel == null)
            throw new ApplicationException(
                $"{nameof(FinishedParserLinkEntrance)} Channel was null."
            );
        if (_connection == null)
            throw new ApplicationException(
                $"{nameof(FinishedParserLinkEntrance)} Connection was null."
            );
        byte[] body = ea.Body.ToArray();
        FinishedParserLinkMessage? message = JsonSerializer.Deserialize<FinishedParserLinkMessage>(
            body
        );
        if (message != null)
        {
            try
            {
                IFinishedParserLinkStorage storage = new NpgSqlFinishedParserLinkStorage(
                    dataSource
                );
                ParserLinkToFinish link = await storage.Fetch(
                    message.ParserName,
                    message.LinkName,
                    message.ParserType
                );
                FinishedParserLink finished = link.Finish(message.TotalElapsedSeconds);
                await finished.Save(storage);
                logger.Information(
                    "Finish parser link: {Name} {Type} {ParserName}.",
                    finished.LinkName,
                    finished.ParserType,
                    finished.ParserName
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
