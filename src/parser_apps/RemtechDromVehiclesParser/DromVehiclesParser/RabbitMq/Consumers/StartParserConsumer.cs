using DromVehiclesParser.Parsers.Database;
using DromVehiclesParser.Parsers.Models;
using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace DromVehiclesParser.RabbitMq.Consumers;

public sealed class StartParserConsumer(
    RabbitMqConnectionSource rabbitMq,
    Serilog.ILogger logger,
    NpgSqlConnectionFactory npgSql) : IConsumer
{
    private const string Exchange = "Drom.Техника";
    private const string Queue = "Drom.Техника.start";
    private const string RoutingKey = "Drom.Техника.start";
    private IChannel? _channel;
    private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel not initialized");
    private Serilog.ILogger Logger { get; } = logger.ForContext<StartParserConsumer>();

    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
    {
        Logger.Information("Initializing channel {Exchange} {Queue} {RoutingKey}", Exchange, Queue, RoutingKey);
        _channel = await TopicConsumerInitialization.InitializeChannel(rabbitMq, Exchange, Queue, RoutingKey, ct);
        Logger.Information("Channel {Exchange} {Queue} {RoutingKey} initialized.", Exchange, Queue, RoutingKey);
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        AsyncEventingBasicConsumer consumer = new(Channel);
        consumer.ReceivedAsync += Handler;
        await Channel.BasicConsumeAsync(queue: Queue, autoAck: false, consumer: consumer, cancellationToken: ct);
    }

    public async Task Shutdown(CancellationToken ct = default)
    {
        Logger.Information("Shutting down channel {Exchange} {Queue} {RoutingKey}", Exchange, Queue, RoutingKey);
        await Channel.CloseAsync(ct);
        Logger.Information("Channel {Exchange} {Queue} {RoutingKey} shut down.", Exchange, Queue, RoutingKey);
    }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
        async (_, @event) =>
        {
            Logger.Information("Received message to start parser.");
            CancellationToken ct = CancellationToken.None;
            try
            {
                await using NpgSqlSession session = new(npgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                ITransactionScope transaction = await transactionSource.BeginTransaction(ct);
                if (await WorkingParser.Exists(session))
                {
                    logger.Warning("""
                                   There is already working parser session.
                                   Cancelling starting new one. 
                                   """);
                    await Channel.BasicAckAsync(@event.DeliveryTag, false);
                    return;
                }

                ParserWorkStage stage = ParserWorkStage.FromDeliverEventArgs(@event).PaginationStage();
                WorkingParser parser = WorkingParser.FromDeliverEventArgs(@event);
                WorkingParserLink[] links = WorkingParserLink.FromDeliverEventArgs(@event);
                await stage.Save(session);
                await parser.Persist(session, ct);
                await links.PersistMany(session);
                Result result = await transaction.Commit(ct);
                if (result.IsFailure)
                {
                    logger.Fatal(result.Error, "Failed to commit transaction");
                    await Channel.BasicAckAsync(@event.DeliveryTag, false);
                    return;
                }

                logger.Information("""
                                   Saved working parser:
                                   Id: {Id}
                                   Domain: {Domain}
                                   Type: {Type}
                                   Links Count: {LinksCount}
                                   """, parser.Id, parser.Domain, parser.Type, links.Length);
                await Channel.BasicAckAsync(@event.DeliveryTag, false);
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Error while handling message to start parser.");
                await Channel.BasicNackAsync(@event.DeliveryTag, false, false);
            }
        };
}