using RemTech.NpgSql.Abstractions;
using RemTech.Outbox.Shared;

namespace Identity.Outbox.Decorators;

public sealed class TransactionalOutboxProcessorWork : IIdentityOutboxProcessorWork
{
    private readonly NpgSqlSession _session;
    private readonly CancellationToken _ct;
    private readonly Serilog.ILogger _logger;
    private readonly IIdentityOutboxProcessorWork _origin;
    
    public async Task<ProcessedOutboxMessages> ProcessMessages()
    {
        try
        {
            ProcessedOutboxMessages result = await _origin.ProcessMessages();
            if (!await _session.Commited(_ct))
                _logger.Error("{Context} Unable to commit transaction", nameof(IIdentityOutboxProcessorWork));
            return result;
        }
        finally
        {
            await _session.DisposeAsync();
        }
    }
    
    public TransactionalOutboxProcessorWork(
        Serilog.ILogger logger,
        NpgSqlSession session, 
        CancellationToken ct,
        IIdentityOutboxProcessorWork origin)
    {
        _logger = logger;
        _session = session;
        _ct = ct;
        _origin = origin;
    }
}