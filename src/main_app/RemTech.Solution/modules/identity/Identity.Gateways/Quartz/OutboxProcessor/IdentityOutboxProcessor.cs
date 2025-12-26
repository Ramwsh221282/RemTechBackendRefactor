using Identity.Gateways.Common;
using Identity.Infrastructure.Outbox;
using Quartz;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace Identity.Gateways.Quartz.OutboxProcessor;

[DisallowConcurrentExecution]
[CronSchedule("0/5 * * * * ?")] // every 5 seconds
public sealed class IdentityOutboxProcessor : ICronScheduleJob
{
    private readonly NpgSqlSession _npgSqlSession;
    private readonly IdentityOutboxMessagesStore _messages;
    private readonly Serilog.ILogger _logger;
    private readonly RabbitMqOutboxMessagePublishersRegistry _publishers;
    private const string Context = nameof(IdentityOutboxProcessor);
    
    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken ct = context.CancellationToken;
        _logger.Information("{Context} starting processing messages.", Context);
        IdentityOutboxMessage[] messages = await _messages.GetUnsentMessages(ct);
        if (messages.Length == 0)
        {
            _logger.Information("{Context} has no pending messages.", Context);
            return;
        }
        
        try
        {
            await _npgSqlSession.GetTransaction(ct);
            List<IdentityOutboxMessage> publishedMessages = [];
            foreach (IdentityOutboxMessage message in messages)
            {
                if (!_publishers.HasPublisherForMessage(message))
                {
                    _logger.Error("{Context} no supported publisher for message: {Type}", Context, message.Type);
                    continue;
                }
                
                PublishedOutboxMessage published = await _publishers.PublishMessage(message, ct);
                publishedMessages.Add(published.Origin());
            }
            
            await _messages.UpdateMessages(publishedMessages.ToArray(), ct);
            if (!await _npgSqlSession.Commited(ct)) throw new InvalidOperationException("Unable to commit transaction");
            _logger.Information("{Context} processed: {Count} messages", Context, messages.Length);
        }
        catch(Exception ex)
        {
            _logger.Error("{Context} error: {Exception}", Context, ex.Message);
        }
    }

    public IdentityOutboxProcessor(
        NpgSqlConnectionFactory npgSqlConnectionFactory,
        Serilog.ILogger logger,
        RabbitMqOutboxMessagePublishersRegistry publishers
        )
    {
        _npgSqlSession = new NpgSqlSession(npgSqlConnectionFactory);
        _messages = new IdentityOutboxMessagesStore(_npgSqlSession);
        _logger = logger;
        _publishers = publishers;
    }
}