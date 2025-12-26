using System.Data.Common;
using Dapper;
using Identity.Domain.Roles.Events;
using Microsoft.EntityFrameworkCore;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers.RoleCreated;

public sealed class RoleCreatedEventHandler(Serilog.ILogger logger, IdentityDbContext context)
    : IDomainEventHandler<RoleCreatedEvent>
{
    private const string Context = nameof(RoleCreatedEvent);

    public async Task<Status> Handle(RoleCreatedEvent @event, CancellationToken ct = default)
    {
        Status result = await InsertRole(@event, ct);
        if (result.IsFailure)
        {
            logger.Error("{Context}. Error: {Error}.", Context, result.Error.ErrorText);
            return result;
        }

        logger.Information(
            "{Context}. Added role: ID: {RoleId}. Name: {Name}",
            Context,
            @event.Info.Id,
            @event.Info.Name
        );

        return Status.Success();
    }

    private async Task<Status> InsertRole(RoleCreatedEvent @event, CancellationToken ct)
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
