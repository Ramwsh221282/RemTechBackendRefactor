using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class AccountHasAccountTickets(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid accountId)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        const string sql = "SELECT COUNT(*) FROM identity_module.account_tickets WHERE account_id = @id";
        CommandDefinition command = new(sql, new { id = accountId });
        int count = await session.QuerySingleRow<int>(command);
        return count > 0;
    }
}