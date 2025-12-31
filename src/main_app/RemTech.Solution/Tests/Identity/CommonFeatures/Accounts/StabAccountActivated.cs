using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class StabAccountActivated(IServiceProvider sp)
{
    public async Task Invoke(Guid id)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        const string sql = "UPDATE identity_module.accounts SET activated = TRUE where id = @id";
        CommandDefinition command = new(sql, new { id });
        await session.Execute(command);
    }
}