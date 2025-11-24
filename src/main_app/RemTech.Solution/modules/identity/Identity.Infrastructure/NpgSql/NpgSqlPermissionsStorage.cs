using Dapper;
using Identity.Core;
using Identity.Core.Permissions.Events;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Identity.Infrastructure.NpgSql;

public sealed class NpgSqlPermissionsStorage(NpgSqlSession session) :
    IEventHandler<PermissionRegistered>,
    IEventHandler<PermissionRenamed>
{
    private readonly Queue<Func<CancellationToken, Task>> _pendingOperations = [];

    public async Task ProcessDatabaseOperations(CancellationToken ct)
    {
        while (_pendingOperations.Count > 0)
        {
            Func<CancellationToken, Task> operation = _pendingOperations.Dequeue();
            await operation(ct);
        }
    }
    
    public void ReactOnEvent(PermissionRegistered @event)
    {
        const string sql =
            """
            INSERT INTO identity_module.permissions(id, name)
            VALUES (@id, @name)
            """;
        _pendingOperations.Enqueue(async ct =>
        {
            await ValidateNameUniquesness(ct, @event.Name);
            CommandDefinition command = new(
                sql,
                new
                {
                    id = @event.Id,
                    name = @event.Name
                },
                cancellationToken: ct,
                transaction: session.Transaction);
            await session.Execute(command);
        });
    }

    public void ReactOnEvent(PermissionRenamed @event)
    {
        const string sql = "UPDATE identity_module.permissions SET name = @name WHERE id = @id";
        _pendingOperations.Enqueue(async ct =>
        {
            await ValidateNameUniquesness(ct, @event.NewName);
            CommandDefinition command = new(sql, new
            {
                id = @event.Id,
                name = @event.NewName
            });
            await session.Execute(command);
        });
    }

    private async Task ValidateNameUniquesness(CancellationToken ct, string name)
    {
        CommandDefinition command = new(
            "SELECT COUNT(*) FROM identity_module.permissions WHERE name = @name",
            new { name },
            cancellationToken: ct,
            transaction: session.Transaction
        );
        int count = await session.QuerySingleRow<int>(command);
        if (count > 0) throw ErrorException.Conflict($"Разрешение с названием: {name} уже существует.");
    }
}