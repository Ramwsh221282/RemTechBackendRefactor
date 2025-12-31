using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.RabbitMq.Consumers;

public sealed class ParserStartConsumer(
    RabbitMqConnectionSource connectionSource,
    NpgSqlConnectionFactory npgSql,
    Serilog.ILogger logger) : IConsumer
{
    private const string ExchangeName = "Avito.Техника";
    private const string QueueName = "Avito.Техника.start";
    private const string RoutingKey = "Avito.Техника.start";
    private IChannel? _channel;
    private IChannel Channel => _channel ?? throw new InvalidOperationException("Channel not initialized");
    private Serilog.ILogger Logger { get; } = logger.ForContext<ParserStartConsumer>();
    
    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default)
    {
        Logger.Information("Initializing channel for {ExchangeName} {QueueName} {RoutingKey}", ExchangeName, QueueName, RoutingKey);
        _channel = await TopicConsumerInitialization.InitializeChannel(connectionSource, ExchangeName, QueueName, RoutingKey, ct);
        Logger.Information("Channel initialized for {ExchangeName} {QueueName} {RoutingKey}", ExchangeName, QueueName, RoutingKey);
    }

    public Task StartConsuming(CancellationToken ct = default)
    {
        Logger.Information("Starting consuming for {ExchangeName} {QueueName} {RoutingKey}", ExchangeName, QueueName, RoutingKey);
        AsyncEventingBasicConsumer consumer = new(Channel);
        consumer.ReceivedAsync += Handler;
        return Channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct
            );
    }

    public Task Shutdown(CancellationToken ct = default)
    {
        Logger.Information("Shutting down consuming for {ExchangeName} {QueueName} {RoutingKey}", ExchangeName, QueueName, RoutingKey);
        return Channel.CloseAsync(ct);
    }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler => async (_, @event) =>
    {
        Logger.Information("Received message to start parser.");
        try
        {
            await using NpgSqlSession session = new(npgSql);
            NpgSqlTransactionSource transactionSource = new(session, Logger);
            await using ITransactionScope txn = await transactionSource.BeginTransaction(CancellationToken.None);

            if (await ProcessingParser.HasAny(session))
            {
                Logger.Information("There is already a parser in progress. Declining.");
                await Channel.BasicAckAsync(@event.DeliveryTag, false);
                return;
            }

            ProcessingParser parser = ProcessingParser.FromDeliverEventArgs(@event);
            ProcessingParserLink[] links = ProcessingParserLink.FromDeliverEventArgs(@event);
            ParserWorkStage stage = ParserWorkStage.PaginationStage();
            await stage.Persist(session);
            await parser.Persist(session);
            await links.PersistMany(session);
            Result commit = await txn.Commit();

            if (commit.IsFailure)
            {
                Logger.Error(commit.Error, "Error at committing transaction");
            }
            else
            {
                Logger.Information(
                    """
                    Added parser to process:
                    Domain: {domain}
                    Type: {type}
                    Id: {id}
                    Stage: {Stage}
                    Links: {Count}
                    """,
                    parser.Domain,
                    parser.Type,
                    parser.Id,
                    stage.Name,
                    links.Length
                );
            }
            await Channel.BasicAckAsync(@event.DeliveryTag, false);
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Error processing message");
            await Channel.BasicNackAsync(@event.DeliveryTag, false, false);
        }
    };
}