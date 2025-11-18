using RemTech.NpgSql.Abstractions;
using RemTech.RabbitMq.Abstractions;

namespace Identity.Outbox;

public sealed class IdentityOutboxProcessorWork : IDisposable, IAsyncDisposable
{
    private readonly IdentityOutboxStorage _storage;
    private readonly RabbitMqConnectionSource _connectionSource;
    private readonly NpgSqlSession _session;
    private readonly CancellationToken _ct = CancellationToken.None;

    public async Task ProcessMessages()
    {
        Console.WriteLine("Identity outbox processor work started.");
        await _session.GetTransaction(_ct);
        IEnumerable<IdentityOutboxMessage> messages = await RecieveMessages(50);
        IdentityMessageProcessingResult result = await PublishMessages(messages);
        await result.RefreshMessagesInDatabase(_storage, _ct);
        if (!await _session.Commited(_ct))
            throw new ApplicationException($"Unable to process {nameof(IdentityOutboxProcessorWork)} transaction");
        Console.WriteLine("Identity outbox processor work finished.");
    }

    private async Task<IEnumerable<IdentityOutboxMessage>> RecieveMessages(int maxAmount)
    {
        return await _storage.GetPendingMessages(maxAmount, _ct);
    }

    private async Task<IdentityMessageProcessingResult> PublishMessages(IEnumerable<IdentityOutboxMessage> messages)
    {
        IdentityMessageProcessingResult result = new();
        foreach (IdentityOutboxMessage message in messages)
        {
            string routingKey = message.RoutingKey;
            string exchange = message.Exchange;
            string body = message.Body;
            await using RabbitMqPublisher publisher = await _connectionSource.CreatePublisher(exchange, routingKey, _ct);
            await publisher.Publish(body, _ct);
            result.ResolveProcessing(await publisher.EnsureMessageDelivered(30), message);
        }

        return result;
    }
    
    public IdentityOutboxProcessorWork(
        NpgSqlSession session, 
        IdentityOutboxStorage storage, 
        RabbitMqConnectionSource connectionSource)
    {
        _storage = storage;
        _connectionSource = connectionSource;
        _session = session;
    }

    public void Dispose()
    {
        _session.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _session.DisposeAsync();
    }
}