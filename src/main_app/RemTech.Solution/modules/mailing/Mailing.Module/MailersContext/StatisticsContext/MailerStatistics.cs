using Dapper;
using Mailing.Module.Traits;
using Serilog;

namespace Mailing.Module.MailersContext.StatisticsContext;

internal sealed class MailerStatistics(int sendLimit, int currentSend)
    : ISnapshottable<MailerStatisticsSnapshot>,
        ILoggable,
        IWriterTo<DynamicParameters>
{
    public void Log(ILogger logger)
    {
        logger.Information(
            """
            Mailer statistics:  
            Send limit: {Limit}.
            Current send: {Current}.
            """, sendLimit, currentSend);
    }

    public MailerStatisticsSnapshot Snapshotted()
    {
        return new MailerStatisticsSnapshot(sendLimit, currentSend);
    }

    public void Write(DynamicParameters target)
    {
        Snapshotted().Write(target);
    }
}