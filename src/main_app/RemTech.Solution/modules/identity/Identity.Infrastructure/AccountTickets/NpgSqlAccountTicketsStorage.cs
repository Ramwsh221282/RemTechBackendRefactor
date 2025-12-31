using System.Data;
using Identity.Application.AccountTickets;
using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;

namespace Identity.Infrastructure.AccountTickets;

public sealed class NpgSqlAccountTicketsStorage(NpgSqlSession session) : IAccountTicketsStorage
{
    public async Task Persist(IAccountTicket instance, CancellationToken ct = default)
    {
        const string sql =
            """
            INSERT INTO identity_module.account_tickets
            (id, account_id, type, payload, created, finished)
            VALUES
            (@id, @account_id, @type, @payload::jsonb, @created, @finished)
            """;
        DynamicParameters parameters = FillParameters(instance.Representation());
        await session.Execute(parameters, sql, ct);
    }
    
    public async Task<IAccountTicket?> Fetch(AccountTicketQueryArgs args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(args);
        string lockClause = LockClause(args);
        string sql =
            $"""
            SELECT * FROM identity_module.account_tickets
            {filterSql}
            {lockClause}
            """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlAccountTicketRow? row = await session.QueryMaybeRow<NpgSqlAccountTicketRow?>(command);
        return row?.ToAccountTicket();
    }

    private static string LockClause(AccountTicketQueryArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }
    
    private static (DynamicParameters parameters, string filterSql) WhereClause(AccountTicketQueryArgs args)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (args.AccountId.HasValue)
        {
            filters.Add("account_id=@account_id");
            parameters.Add("@account_id", args.AccountId.Value, DbType.Guid);
        }

        if (args.Id.HasValue)
        {
            filters.Add("id=@id");
            parameters.Add("@id", args.Id.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(args.Type))
        {
            filters.Add("type=@type");
            parameters.Add("@type", args.Type, DbType.String);
        }

        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, sql);
    }
    
    private static DynamicParameters FillParameters(AccountTicketData data)
    {
        DynamicParameters parameters = new();
        parameters.Add("@id", data.Id, DbType.Guid);
        parameters.Add("@account_id", data.AccountId, DbType.Guid);
        parameters.Add("@type", data.Type, DbType.String);
        parameters.Add("@payload", data.Payload);
        parameters.Add("@created", data.Created);
        if (data.Finished.HasValue) parameters.Add("@finished", data.Finished.Value, DbType.DateTime);
        else parameters.Add("@finished", null, DbType.DateTime);
        return parameters;
    }

    private sealed record NpgSqlAccountTicketRow(
        Guid Id,
        Guid AccountId,
        string Type,
        string Payload,
        DateTime Created,
        DateTime? Finished)
    {
        private AccountTicketData ToTicketData()
        {
            return new AccountTicketData(Id, AccountId, Type, Payload, Created, Finished);
        }
        
        public IAccountTicket ToAccountTicket()
        {
            return new AccountTicket(ToTicketData());
        }
    }
}