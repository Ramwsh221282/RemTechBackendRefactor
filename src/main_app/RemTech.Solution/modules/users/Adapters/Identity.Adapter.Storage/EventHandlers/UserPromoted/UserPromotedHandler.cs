using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages;
using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.EventHandlers.UserPromoted;

public sealed class UserPromotedHandler(PostgresDatabase database, Serilog.ILogger logger)
    : IDomainEventHandler<Domain.Users.Events.UserPromoted>
{
    private const string Context = nameof(UserPromotedHandler);

    public async Task<Status> Handle(
        Domain.Users.Events.UserPromoted @event,
        CancellationToken ct = default
    )
    {
        using IDbConnection connection = await database.ProvideConnection(ct: ct);
        using IDbTransaction transaction = connection.ProvideTransaction();
        UsersStorage storage = new(database);
        try
        {
            Status<User> target = await storage.GetUserWithBlock(
                connection,
                transaction,
                @event.UserId,
                ct
            );
            if (target.IsFailure)
                return Status.Failure(target.Error);

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

    private async Task<Status> InsertUserRoleEntry(
        IDbConnection connection,
        IDbTransaction transaction,
        Domain.Users.Events.UserPromoted @event,
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
