using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Features.CompleteScraperJournal;
using Scrapers.Module.Features.CreateNewParser.RabbitMq;
using Scrapers.Module.Features.FinishParser.Database;
using Scrapers.Module.Features.FinishParser.Models;
using Scrapers.Module.ParserStateCache;
using StackExchange.Redis;

namespace Scrapers.Module.Features.FinishParser.Entrance;

internal sealed class FinishParserEntrance(
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource,
    ConnectionFactory connectionFactory,
    ConnectionMultiplexer multiplexer
) : BackgroundService
{
    private const string Exchange = "scrapers";
    private const string Queue = "finish_scraper";
    private readonly ParserStateCachedStorage _stateCache = new(multiplexer);
    private readonly ActiveScraperJournalsCache _journalsCache = new(multiplexer);
    private IConnection? _connection;
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken ct)
    {
        logger.Information("{Service} starting...", nameof(FinishParserEntrance));
        _connection = await connectionFactory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(
            Exchange,
            ExchangeType.Direct,
            false,
            false,
            cancellationToken: ct
        );
        await _channel.QueueDeclareAsync(
            Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct
        );
        await _channel.QueueBindAsync(Queue, Exchange, Queue, cancellationToken: ct);
        await base.StartAsync(ct);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
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
            cancellationToken: ct
        );
        ct.ThrowIfCancellationRequested();
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
                await _stateCache.UpdateState(
                    finished.ParserName,
                    finished.ParserType,
                    finished.ParserState
                );
                await new CompleteScraperJournalHandler(dataSource, logger, _journalsCache).Handle(
                    finished.CompleteJournalCommand()
                );
                await _journalsCache.RemoveIdentifier(finished.ParserName, finished.ParserType);
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
