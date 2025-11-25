using Dapper;
using Identity.Core;
using Identity.Core.Accounts.Events;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Identity.Infrastructure.NpgSql;

public sealed class NpgSqlAccountsStorage (
    NpgSqlSession session) :
    IEventHandler<AccountRegisteredEvent>, 
    IEventHandler<AccountEmailChangedEvent>, 
    IEventHandler<AccountPasswordChangedEvent>
{
    private readonly Queue<Func<CancellationToken, Task>> _pendingOperations = [];

    public async Task ProcessDatabaseCommands(CancellationToken ct = default)
    {
        while (_pendingOperations.Count > 0)
        {
            Func<CancellationToken, Task> operation = _pendingOperations.Dequeue();
            await operation(ct);
        }
    }
    
    public void ReactOnEvent(AccountRegisteredEvent @event)
    {
        const string sql =
            """
            INSERT INTO identity_module.accounts
            (id, email, name, password, activated)
            VALUES
            (@id, @email, @name, @password, @activated)
            """;
        
        _pendingOperations.Enqueue(async ct =>
        {
            await ValidateEmailUniquesness(@event.Email, ct);
            await ValidateNameUniquesness(@event.Name, ct);
            await session.Execute(new CommandDefinition(
                sql,
                new
                {
                    id = @event.Id,
                    email = @event.Email,
                    name = @event.Name,
                    password = @event.Password,
                    activated = @event.Activated
                },
                cancellationToken: ct,
                transaction: session.Transaction
                ));
        });
    }

    public void ReactOnEvent(AccountEmailChangedEvent @event)
    {
        const string sql =
            """
            UPDATE identity_module.accounts SET email = @email
            WHERE id = @id AND email = @oldEmail
            """;
        
        _pendingOperations.Enqueue(async ct =>
        {
            await ValidateEmailUniquesness(@event.NewEmail, ct);
            await session.Execute(new CommandDefinition(
                sql,
                new
                {
                    id = @event.Id,
                    email = @event.NewEmail,
                    oldEmail = @event.OldEmail
                },
                cancellationToken: ct,
                transaction: session.Transaction
                ));
        });
    }

    public void ReactOnEvent(AccountPasswordChangedEvent @event)
    {
        const string sql = "UPDATE identity_module.accounts SET password = @password";
        _pendingOperations.Enqueue(async ct =>
        {
            await session.Execute(new CommandDefinition(
                sql,
                new
                {
                    password = @event.NewPassword
                },
                cancellationToken: ct,
                transaction: session.Transaction));
        });
    }
    
    private async Task ValidateNameUniquesness(string name, CancellationToken ct)
    {
        const string sql =
            """
            SELECT COUNT(*) 
            FROM identity_module.accounts 
            WHERE name = @name 
            """;
        CommandDefinition command = new(sql, new { name }, cancellationToken: ct, transaction: session.Transaction);
        int count = await session.QuerySingleRow<int>(command);
        if (count > 0) throw ErrorException.Conflict($"Учетная запись с названием: {name} уже существует.");
    }

    private async Task ValidateEmailUniquesness(string email, CancellationToken ct)
    {
        const string sql =
            """
            SELECT COUNT(*)
            FROM identity_module.accounts 
            WHERE email = @email
            """;
        CommandDefinition command = new(sql, new { email }, cancellationToken: ct, transaction: session.Transaction);
        int count = await session.QuerySingleRow<int>(command);
        if (count > 0) throw ErrorException.Conflict($"Учетная запись с почтой: {email} уже существует.");
    }
}