using System.Data;
using Dapper;
using Identity.Domain.Users.Events;
using Microsoft.EntityFrameworkCore;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.EventHandlers.IdentityUserRemoved;

public sealed class IdentityUserRemovedEventHandler(IdentityDbContext context)
    : IDomainEventHandler<IdentityUserRemovedEvent>
{
    public async Task<Status> Handle(
        IdentityUserRemovedEvent @event,
        CancellationToken ct = default
    )
    {
        const string sql = """
            DELETE FROM users_module.users
            WHERE id = @id
            """;

        CommandDefinition command = new(
            sql,
            new { id = @event.RemovedInfo.UserId },
            cancellationToken: ct
        );

        IDbConnection connection = context.Database.GetDbConnection();

        return await connection.ExecuteAsync(command) == 0
            ? Status.NotFound("Пользователь не был найден при удалении")
            : Status.Success();
    }
}
