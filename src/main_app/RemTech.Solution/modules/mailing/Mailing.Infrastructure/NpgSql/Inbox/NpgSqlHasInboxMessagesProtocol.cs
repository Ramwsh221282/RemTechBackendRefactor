namespace Mailing.Infrastructure.NpgSql.Inbox;

public sealed class NpgSqlHasInboxMessagesProtocol(NpgSqlSession session)
{
    public async Task<bool> Has(CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(*) FROM mailing_module.inbox_messages";
        CommandDefinition command = new(sql, cancellationToken: ct, transaction: session.Transaction);
        int amount = await session.QuerySingleRow<int>(command);
        return amount > 0;
    }
}