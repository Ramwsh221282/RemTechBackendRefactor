using System.Data;
using Dapper;
using Mailing.Module.Traits;

namespace Mailing.Module.MailersContext.StatisticsContext;

internal sealed record MailerStatisticsSnapshot(int SendLimit, int CurrentSend) : Snapshot, IWriterTo<DynamicParameters>
{
    public void Write(DynamicParameters target)
    {
        target.Add("@send_limit", SendLimit, DbType.Int32);
        target.Add("@current_send", CurrentSend, DbType.Int32);
    }
}