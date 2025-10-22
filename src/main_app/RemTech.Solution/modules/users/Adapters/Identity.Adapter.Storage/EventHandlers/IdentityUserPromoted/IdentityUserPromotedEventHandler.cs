using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Extensions;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Events;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.EventHandlers.IdentityUserPromoted;

public sealed class IdentityUserPromotedEventHandler(
    PostgresDatabase database,
    Serilog.ILogger logger
) : IDomainEventHandler<IdentityUserPromotedEvent>
{
    private const string Context = nameof(IdentityUserPromotedEventHandler);

    public async Task<Status> Handle(
        IdentityUserPromotedEvent @event,
        CancellationToken ct = default
    )
    {
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        using IDbTransaction transaction = connection.ProvideTransaction();
        try
        {
            Status<IdentityUser> user = await GetUserWithBlock(connection, transaction, @event, ct);
            if (user.IsFailure)
                return Status.Failure(user.Error);

            await InsertUserRoleEntry(connection, transaction, @event, ct);
            transaction.Commit();
            return Status.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.Error("{Context}. Error: {ErrorMessage}.", Context, ex);
            return Status.Internal("Ошибка транзакции.");
        }
    }

    private async Task<Status<IdentityUser>> GetUserWithBlock(
        IDbConnection connection,
        IDbTransaction transaction,
        IdentityUserPromotedEvent @event,
        CancellationToken ct
    )
    {
        IdentityUser? user = await connection.QueryUser(
            ["u.id = @userId"],
            new { userId = @event.UserId },
            transaction: transaction,
            true,
            ct
        );

        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }

    private async Task<Status> InsertUserRoleEntry(
        IDbConnection connection,
        IDbTransaction transaction,
        IdentityUserPromotedEvent @event,
        CancellationToken ct
    )
    {
        const string insertSql = """
            INSERT INTO users_module.user_roles
            (user_id, role_id)
            VALUES
            (@user_id, @role_id)
            ON CONFLICT (user_id, role_id) DO NOTHING
            """;

        CommandDefinition command = new(
            insertSql,
            new { user_id = @event.UserId, role_id = @event.RoleId },
            cancellationToken: ct,
            transaction: transaction
        );

        return await connection.ExecuteAsync(command) == 0
            ? Status.Conflict("Такая роль уже есть у пользователя")
            : Status.Success();
    }
}
