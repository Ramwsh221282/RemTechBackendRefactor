using RemTech.Outbox.Shared;

namespace Tickets.Outbox;

public sealed class LoggingTicketsOutboxJobMethod(
    Serilog.ILogger logger, 
    ITicketsOutboxJobMethod origin)
    : ITicketsOutboxJobMethod
{
    private const string Context = nameof(TicketsOutboxJobMethod);
    
    public async Task<ProcessedOutboxMessages> ProcessMessages(CancellationToken ct)
    {
        logger.Information("{Context} processing outbox messages.", Context);
        try
        {
            ProcessedOutboxMessages result = await origin.ProcessMessages(ct);
            logger.Information("{Context} processed messages count: {Count}", Context, result.ProcessedCount);
            return result;
        }
        catch(Exception ex)
        {
            logger.Error("{Context} failed processing outbox messages.", Context);
            logger.Error("{Context} {Ex}", Context, ex);
            return new ProcessedOutboxMessages();
        }
    }
}