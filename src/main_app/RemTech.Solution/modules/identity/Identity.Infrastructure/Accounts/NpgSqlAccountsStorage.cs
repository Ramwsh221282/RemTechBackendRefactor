using System.Data;
using Dapper;
using Identity.Application.Accounts;
using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Identity.Infrastructure.Accounts;

public sealed class NpgSqlAccountsStorage(NpgSqlSession session) : IAccountsStorage
{
    public async Task Persist(IAccount instance, CancellationToken ct = default)
    {
        const string sql =
            """
            INSERT INTO identity_module.accounts
            (id, name, email, password, activated)
            VALUES
            (@id, @name, @email, @password, @activated)
            """;
        DynamicParameters parameters = FillParameters(instance);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }
    
    public async Task<IAccount?> Fetch(AccountQueryArgs args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string whereClause) filterClause = FilterClauses(args);
        string lockClause =  LockClause(args);
        string sql =
            $"""
             SELECT * FROM identity_module.accounts
             {filterClause.whereClause}
             {lockClause}
             LIMIT 1
             """;
        CommandDefinition command = session.FormCommand( sql, filterClause.parameters, ct);
        NpgSqlAccountRow? row = await session.QueryMaybeRow<NpgSqlAccountRow>(command);
        return row?.FormAccount();
    }
    
    public async Task Remove(IAccount instance, CancellationToken ct = default)
    {
        const string sql =
            """
            DELETE FROM identity_module.accounts
            WHERE id = @id
            """;
        DynamicParameters parameters = FillParameters(instance);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    public async Task Update(IAccount instance, CancellationToken ct = default)
    {
        const string sql =
            """
            UPDATE identity_module.accounts
            SET
                name = @name,
                email = @email,
                password = @password,
                activated = @activated
            WHERE id = @id
            """;
        DynamicParameters parameters = FillParameters(instance);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    private static DynamicParameters FillParameters(IAccount account)
    {
        AccountData data = account.Represent();
        DynamicParameters parameters = new();
        parameters.Add("@id", data.Id, DbType.Guid);
        parameters.Add("@name", data.Name, DbType.String);
        parameters.Add("@email", data.Email, DbType.String);
        parameters.Add("@password", data.Password, DbType.String);
        parameters.Add("@activated", data.Activated, DbType.Boolean);
        return parameters;
    }
    
    private static (DynamicParameters parameters, string sql) FilterClauses(AccountQueryArgs args)
    {
        DynamicParameters parameters = new();
        List<string> filters = [];
        if (args.Id.HasValue)
        {
            parameters.Add("@id", args.Id.Value, DbType.Guid);
            filters.Add("id=@id");
        }

        if (!string.IsNullOrWhiteSpace(args.Name))
        {
            parameters.Add("@name", args.Name, DbType.String);
            filters.Add("name=@name");
        }

        if (!string.IsNullOrWhiteSpace(args.Email))
        {
            parameters.Add("@email", args.Email, DbType.String);
            filters.Add("email=@email");
        }

        string resultSql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, resultSql);
    }

    private static string LockClause(AccountQueryArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }

    private sealed class NpgSqlAccountRow(
        Guid Id, 
        string Name, 
        string Email, 
        string Password, 
        bool Activated)
    {
        private AccountData FormAccountData()
        {
            return new AccountData(Id, Name, Email, Password, Activated);
        }

        public IAccount FormAccount()
        {
            return new Account(FormAccountData());
        }
    }
}