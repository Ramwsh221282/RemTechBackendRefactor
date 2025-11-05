using System.Data;
using Dapper;
using Mailing.Domain.Postmans.Storing;
using RemTech.Core.Shared.Primitives.Async;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Adapters.Storage.Postmans.Storage;

/* CREATE TABLE mailing_module.postmans
// (
//     id            UUID PRIMARY KEY,
//     email         varchar(255) not null UNIQUE,
//     password      varchar(512) not null,
//     current_sent  INT          NOT NULL,
//     current_limit INT          NOT NULL
 ) */
public sealed class NpgSqlPostmansStorage(
    PostgresDatabase database,
    DelayedAsyncAction action,
    CancellationToken ct = default)
    : IPostmansStorage
{
    private readonly DynamicParameters _parameters = new();

    public void Save(Action<IPostmanMetadataStorage, IPostmanStatisticsStorage> saveDelegate)
    {
        saveDelegate(this, this);
        ExecuteSaving();
    }

    public void Save(int sendLimit, int currentSend)
    {
        _parameters.Add("send_limit", sendLimit, DbType.Int32);
        _parameters.Add("send_current", currentSend, DbType.Int32);
    }

    public void Save(Guid id, string email, string password)
    {
        _parameters.Add("id", id, DbType.Guid);
        _parameters.Add("email", email, DbType.String);
        _parameters.Add("password", password, DbType.String);
    }

    private void ExecuteSaving() =>
        action.Enqueue(new AsyncAction(async () =>
        {
            using DbPostman postman = new(database, _parameters, ct);
            await postman.Save();
        }));
}