using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages;
using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.EventHandlers.UserDemoted;

public sealed class UserDemotedHandler(Serilog.ILogger logger, PostgresDatabase database)
    : IDomainEventHandler<Domain.Users.Events.UserDemoted>
{
    private const string Context = nameof(UserDemotedHandler);

    public async Task<Status> Handle(
        Domain.Users.Events.UserDemoted @event,
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

            Status deletion = await DeleteUserRoleEntry(connection, transaction, @event, ct);
            if (deletion.IsFailure)
                return deletion;

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

    private async Task<Status> DeleteUserRoleEntry(
        IDbConnection connection,
        IDbTransaction transaction,
        Domain.Users.Events.UserDemoted @event,
        CancellationToken ct
    )
    {
        const string insertSql = """
            DELETE FROM users_module.user_roles
            WHERE user_id = @user_id AND role_id = @role_id
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
