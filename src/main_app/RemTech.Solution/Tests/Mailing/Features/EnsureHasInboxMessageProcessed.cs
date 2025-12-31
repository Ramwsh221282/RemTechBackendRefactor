using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Mailing.Features;

public sealed class EnsureHasInboxMessageProcessed(IServiceProvider sp)
{
    public async Task<bool> Invoke()
    {
        CancellationToken ct = CancellationToken.None;
        const string sql = "SELECT EXISTS(SELECT 1 FROM mailing_module.inbox_messages WHERE has_processed = TRUE)";
        CommandDefinition command = new(sql);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        bool exists = await session.QuerySingleRow<bool>(command);
        return exists;
    }
}