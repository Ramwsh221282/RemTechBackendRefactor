using Dapper;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Tests.Mailing.Features;

public sealed class EnsureHasInboxMessage(IServiceProvider sp)
{
    public async Task<bool> HasInboxMessage()
    {
        CancellationToken ct = CancellationToken.None;
        const string sql = "SELECT EXISTS(SELECT 1 FROM mailing_module.inbox_messages)";
        CommandDefinition command = new(sql);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        bool exists = await session.QuerySingleRow<bool>(command);
        return exists;
    }
}