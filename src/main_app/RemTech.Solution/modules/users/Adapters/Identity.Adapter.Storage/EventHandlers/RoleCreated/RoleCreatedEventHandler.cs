using System.Data.Common;
using Dapper;
using Identity.Domain.Roles.Events;
using Microsoft.EntityFrameworkCore;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers.RoleCreated;

public sealed class RoleCreatedEventHandler(IdentityDbContext context)
    : IDomainEventHandler<RoleCreatedEvent>
{
    public async Task<Status> Handle(RoleCreatedEvent @event, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO users_module.roles
            (id, name)
            VALUES
            (@id, @name)
            ON CONFLICT (name) DO NOTHING
            """;

        RoleEventArgs ea = @event.Info;

        CommandDefinition command = new(
            sql,
            new { id = ea.Id, name = ea.Name },
            cancellationToken: ct
        );

        await using DbConnection connection = context.Database.GetDbConnection();
        int affected = await connection.ExecuteAsync(command);

        return affected == 0
            ? Status.Conflict($"Роль с названием: {ea.Name} уже существует.")
            : Status.Success();
    }
}
