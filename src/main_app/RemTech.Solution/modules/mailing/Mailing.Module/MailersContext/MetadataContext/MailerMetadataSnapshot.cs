using System.Data;
using Dapper;
using Mailing.Module.Traits;

namespace Mailing.Module.MailersContext.MetadataContext;

internal sealed record MailerMetadataSnapshot(Guid Id, string Email, string Password) : Snapshot,
    IWriterTo<DynamicParameters>
{
    public void Write(DynamicParameters target)
    {
        target.Add("@id", Id, DbType.Guid);
        target.Add("@email", Email, DbType.String);
        target.Add("@password", Password, DbType.String);
    }
}