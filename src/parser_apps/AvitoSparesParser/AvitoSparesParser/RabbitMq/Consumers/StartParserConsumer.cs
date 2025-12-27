using AvitoSparesParser.ParserStartConfiguration;
using AvitoSparesParser.ParserStartConfiguration.Extensions;
using AvitoSparesParser.ParsingStages;
using AvitoSparesParser.ParsingStages.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace AvitoSparesParser.RabbitMq.Consumers;

public sealed class StartParserConsumer(
    NpgSqlConnectionFactory npgSql,
    Serilog.ILogger logger,
    RabbitMqConnectionSource rabbitMq
    ) : IConsumer
{
    private const string Exchange = "Avito.Запчасти";
    private const string Queue = "Avito.Запчасти.start";
    private const string RoutingKey = "Avito.Запчасти.start";
    
    private IChannel? _channel;
    private Serilog.ILogger Logger { get; } = logger.ForContext<StartParserConsumer>();
    private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel not initialized.");
    
    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
    {
        Logger.Information("Starting channel initialization {Exchange} {Queue} {RoutingKey}", Exchange, Queue, RoutingKey);
        _channel = await TopicConsumerInitialization.InitializeChannel(rabbitMq, Exchange, Queue, RoutingKey, ct);
        Logger.Information("Channel {Exchange} {Queue} {RoutingKey} initialized.", Exchange, Queue, RoutingKey);
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        AsyncEventingBasicConsumer consumer = new(Channel);
        consumer.ReceivedAsync += Handler;
        Logger.Information("Starting consuming.");
        await Channel.BasicConsumeAsync(queue: Queue, autoAck: false, consumer: consumer, cancellationToken: ct);
    }

    public async Task Shutdown(CancellationToken ct = default)
    {
        Logger.Information("Shutting down channel.");
        await Channel.CloseAsync(ct);
        Logger.Information("Channel closed.");
    }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler => async (_, @event) =>
    {
        Logger.Information("Received message to invoke parser process.");
        try
        {
            await using NpgSqlSession session = new(npgSql);
            NpgSqlTransactionSource source = new(session);
            ITransactionScope scope = await source.BeginTransaction();

            if (await ProcessingParser.Exists(session))
            {
                Logger.Information("There is already processing parser in process.");
                await Channel.BasicAckAsync(@event.DeliveryTag, false);
                return;
            }

            ProcessingParser parser = ProcessingParser.FromDeliverEventArgs(@event);
            ProcessingParserLink[] links = IEnumerable<ProcessingParserLink>.ArrayFromDeliverEventArgs(@event);
            if (links.Length == 0)
            {
                Logger.Information("No links for parser {Domain} {Type}", parser.Domain, parser.Type);
                await Channel.BasicAckAsync(@event.DeliveryTag, false);
                return;
            }
            ParsingStage stage = ParsingStage.PaginationFromParser(parser);

            await stage.Save(session);
            await parser.Add(session);
            await links.AddMany(session);
                    
            Result commit = await scope.Commit();
            if (commit.IsFailure)
            {
                Logger.Error(commit.Error, "Failed to commit transaction for parser {Domain} {Type}", parser.Domain, parser.Type);
                return;
            }

            Logger.Information("Parser {Domain} {Type} has been registered with links count: {Count}.",
                parser.Domain, parser.Type, links.Length);
            await Channel.BasicAckAsync(@event.DeliveryTag, false);
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Error while invoking parser process.");
            await Channel.BasicNackAsync(@event.DeliveryTag, false, false);
        }
    };
}