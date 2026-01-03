using System.Data;
using System.Data.Common;
using Dapper;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.Permissions;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Accounts;

public sealed class AccountsRepository(NpgSqlSession session, IAccountsModuleUnitOfWork unitOfWork) : IAccountsRepository 
{
    private NpgSqlSession Session { get; } = session;
    private IAccountsModuleUnitOfWork UnitOfWork { get; } = unitOfWork;

    public async Task Add(Account account, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO identity_module.accounts
                           (id, email, password, login, activation_status)
                           VALUES
                           (@id, @email, @password, @login, @activation_status)
                           """;

        CommandDefinition command = Session.FormCommand(sql, GetParameters(account), ct);
        await Session.Execute(command);
    }

    public async Task<bool> Exists(AccountSpecification specification, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(specification);
        string sql = $"SELECT EXISTS (SELECT 1 FROM identity_module.accounts {filterSql}";
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        return await session.QuerySingleRow<bool>(command);
    }

    public async Task<Result<Account>> Get(AccountSpecification specification, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(specification);
        string sql = $"""
                     SELECT
                     a.id as account_id,
                     a.email as account_email,
                     a.password as account_password,
                     a.login as account_login,
                     a.activation_status as account_activation_status,
                     ap.id as account_permission_id,
                     ap.name as account_permission_name,
                     ap.description as account_permission_description
                     FROM identity_module.accounts a
                     LEFT JOIN identity_module.account_permissions ap ON ap.account_id=a.id
                     {filterSql}
                     """;
        
        CommandDefinition command = Session.FormCommand(sql, parameters, ct);
        NpgsqlConnection connection = await session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        Account? account = await MapFromReader(reader, ct);
        if (account is null) return Error.NotFound("Учетная запись не найдена.");
        if (specification.LockRequired) await BlockAccount(account, ct);
        UnitOfWork.Track([account]);
        return account;
    }

    private async Task<Account?> MapFromReader(DbDataReader reader, CancellationToken ct = default)
    {
        Dictionary<Guid, Account> mappings = [];
        while (await reader.ReadAsync())
        {
            Guid id = reader.GetValue<Guid>("account_id");
            if (!mappings.TryGetValue(id, out Account? account))
            {
                string email = reader.GetValue<string>("account_email");
                string password = reader.GetValue<string>("account_password");
                string login = reader.GetValue<string>("account_login");
                bool status = reader.GetValue<bool>("account_activation_status");
                AccountId accountId = AccountId.Create(id);
                account = new Account(
                    accountId,
                    AccountEmail.Create(email),
                    AccountPassword.Create(password),
                    AccountLogin.Create(login),
                    AccountActivationStatus.Create(status),
                    AccountPermissionsCollection.Empty(accountId)
                );
                mappings.Add(id, account);
            }
            
            if (await reader.IsDBNullAsync("account_permission_id")) continue;
            
            Guid permissionId = reader.GetValue<Guid>("account_permission_id");
            string permissionName = reader.GetValue<string>("account_permission_name");
            string permissionDescription = reader.GetValue<string>("account_permission_description");
            
            Permission permission = new(
                PermissionId.Create(permissionId), 
                PermissionName.Create(permissionName),
                PermissionDescription.Create(permissionDescription));
            
            account.Permissions.Add(permission);
        }
        
        return mappings.Count == 0 ? null : mappings.First().Value;
    }
    
    private (DynamicParameters parameters, string filterSql) WhereClause(AccountSpecification specification)
    {
        DynamicParameters parameters = new();
        List<string> filters = [];

        if (specification.Id.HasValue)
        {
            parameters.Add("@accountId", specification.Id.Value, DbType.Guid);
            filters.Add("a.id=@accountId");
        }

        if (!string.IsNullOrWhiteSpace(specification.Login))
        {
            parameters.Add("@login", specification.Login, DbType.String);
            filters.Add("a.login=@login");
        }

        if (!string.IsNullOrWhiteSpace(specification.Email))
        {
            parameters.Add("@email", specification.Email, DbType.String);
            filters.Add("a.email=@email");
        }
        
        return (parameters, filters.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", filters)}");
    }

    private async Task BlockAccount(Account account, CancellationToken ct = default)
    {
        const string sql = """
                           WITH account_permissions AS (
                               SELECT ap.id, ap.account_id
                               FROM identity_module.account_permissions ap
                               WHERE ap.account_id = @accountId
                               FOR UPDATE
                           )
                           SELECT a.id, ap.id AS permission_id
                           FROM identity_module.accounts a
                           LEFT JOIN account_permissions ap
                               ON ap.account_id = a.id
                           WHERE a.id = @accountId
                           FOR UPDATE OF a
                           """;
        DynamicParameters parameters = new();
        parameters.Add("@accountId", account.Id.Value, DbType.Guid);
        CommandDefinition command = new(sql, parameters, transaction: Session.Transaction, cancellationToken: ct);
        await Session.Execute(command);
    }
    
    private object GetParameters(Account account)
    {
        return new
        {
            id = account.Id.Value,
            email = account.Email.Value,
            password = account.Password.Value,
            login = account.Login.Value,
            activationStatus = account.ActivationStatus.Value
        };
    }
}