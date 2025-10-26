using System.Data;
using System.Text;
using Cleaners.Domain.Cleaners.Ports.Outbox;
using Dapper;
using RabbitMQ.Client;

namespace Cleaners.Adapter.Outbox;

public sealed class TransactionalOutboxMessage
{
    private readonly CleanerOutboxMessage _message;
    private readonly IChannel _channel;
    private readonly IDbTransaction _transaction;
    private readonly IDbConnection _connection;

    public TransactionalOutboxMessage(
        CleanerOutboxMessage message,
        IChannel channel,
        IDbTransaction transaction,
        IDbConnection connection
    )
    {
        _message = message;
        _channel = channel;
        _transaction = transaction;
        _connection = connection;
    }

    public async Task Publish()
    {
        _message.ProcessedAttempts += 1;
        string content = _message.Content;
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        await _channel.BasicPublishAsync("cleaner_processor", _message.Type, body: bytes);
        await UpdateAsProcessed();
    }

    public async Task UpdateAsNotProcessed(string error)
    {
        _message.Processed = null;
        const string sql = """
                           UPDATE cleaners_module.outbox SET 
                               processed = @processed,
                               last_error = @error 
                           WHERE id = @id
                           """;

        var command = new CommandDefinition(
            sql,
            new
            {
                id = _message.Id,
                processed = _message.Processed,
                error,
            },
            transaction: _transaction
        );

        await _connection.ExecuteAsync(command);
        _transaction.Commit();
    }

    private async Task UpdateAsProcessed()
    {
        _message.Processed = DateTime.UtcNow;
        const string sql = """
                           UPDATE cleaners_module.outbox 
                           SET 
                               processed_attempts = @processed_attempts, 
                               processed = @processed 
                           WHERE id = @id
                           """;

        var command = new CommandDefinition(
            sql,
            new
            {
                id = _message.Id,
                processed_attempts = _message.ProcessedAttempts,
                processed = _message.Processed,
            },
            transaction: _transaction
        );

        await _connection.ExecuteAsync(command);
        _transaction.Commit();
    }
}