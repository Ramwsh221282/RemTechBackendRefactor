using RemTech.NpgSql.Abstractions;
using RemTech.RabbitMq.Abstractions.Publishers;
using RemTech.RabbitMq.Abstractions.Publishers.Topic;

namespace Identity.Outbox;

public sealed class IdentityOutboxProcessorWork
{
    private readonly NpgSqlConnectionFactory _dbConnectionFactory;
    private readonly RabbitMqPublishers _publishers;
    private readonly CancellationToken _ct = CancellationToken.None;
    private readonly Serilog.ILogger _logger;
    private const string Context = nameof(IdentityOutboxProcessorWork);

    public async Task ProcessMessages()
    {
        NpgSqlSession session = new(_dbConnectionFactory);
        try
        {
            _logger.Information("Beginning outbox processor work.");
            IdentityOutboxStorage storage = IdentityOutboxStorage.Create(session);
            await session.GetTransaction(_ct);
            IEnumerable<IdentityOutboxMessage> messages = await RecieveMessages(50, storage);
            IdentityMessageProcessingResult result = await PublishMessages(messages);
            await result.RemoveProcessedMessages(storage, _ct);
            if (!await session.Commited(_ct))
                _logger.Error("{Context} Unable to commit transaction", Context);
        }
        catch(Exception ex)
        {
            _logger.Error(ex, "{Context} failed", Context);
        }
        finally
        {
            await session.DisposeAsync();
        }
        _logger.Information("Outbox processor work completed.");
    }

    private async Task<IEnumerable<IdentityOutboxMessage>> RecieveMessages(
        int maxAmount, 
        IdentityOutboxStorage storage)
    {
        return await storage.GetPendingMessages(maxAmount, _ct);
    }

    private async Task<IdentityMessageProcessingResult> PublishMessages(IEnumerable<IdentityOutboxMessage> messages)
    {
        IdentityMessageProcessingResult result = new();
        
        foreach (IdentityOutboxMessage message in messages)
        {
            IRabbitMqPublisher<TopicPublishOptions> publisher = await _publishers.TopicPublisher();
            
            string body = message.Body;
            string queue = message.Queue;
            string exchange = message.Exchange;
            string routingKey = message.RoutingKey;
            
            TopicPublishOptions options = new(body, queue, exchange, routingKey);
            await publisher.Publish(options, _ct);
            result.AddProcessedMessage(message);
            
            _logger.Information("""
                                {Context} publsh message info:
                                Queue: {Queue}
                                Exchange: {Exchange}
                                RoutingKey: {RoutingKey}
                                """, Context, queue, exchange, routingKey);
        }
        
        _logger.Information("{Context} Processed messages count: {Count}", Context, result.ProcessedCount);
        return result;
    }
    
    public IdentityOutboxProcessorWork(
        NpgSqlConnectionFactory dbConnectionFactory,
        RabbitMqPublishers publishers,
        Serilog.ILogger logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _publishers = publishers;
        _logger = logger.ForContext(typeof(IdentityOutboxProcessorWork));
    }
}