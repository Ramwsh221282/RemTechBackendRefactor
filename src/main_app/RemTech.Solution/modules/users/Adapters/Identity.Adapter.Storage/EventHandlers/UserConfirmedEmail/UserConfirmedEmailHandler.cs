using System.Data;
using Dapper;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.EventHandlers.UserConfirmedEmail;

public sealed class UserConfirmedEmailHandler(Serilog.ILogger logger, PostgresDatabase database)
    : IDomainEventHandler<Domain.Users.Events.UserConfirmedEmail>
{
    private const string Context = nameof(UserConfirmedEmailHandler);

    public async Task<Status> Handle(
        Domain.Users.Events.UserConfirmedEmail @event,
        CancellationToken ct = default
    )
    {
        using var connection = await database.ProvideConnection(ct);
        using var transaction = connection.ProvideTransaction();

        try
        {
            var ticketExists = await UserWithTicketExists(@event, connection, transaction, ct);
            if (ticketExists.IsFailure)
                return Status.Failure(ticketExists.Error);

            await UpdateUserEmailConfirmation(@event, connection, transaction, ct);
            await DeleteUserTicket(@event, connection, transaction, ct);

            transaction.Commit();

            logger.Information(
                "{Context}: User: {Id} confirmed ticket {TicketId}.",
                Context,
                @event.UserId,
                @event.TicketId
            );

            return Status.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.Error("{Context}. Error: {Exception}.", Context, ex);
            return Status.Internal("Не удается подтвердить почту пользователя.");
        }
    }

    private async Task<Status> UserWithTicketExists(
        Domain.Users.Events.UserConfirmedEmail @event,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT 1
            FROM users_module.users u
            INNER JOIN users_module.tickets t ON u.id = t.user_id
            WHERE u.id = @user_id AND t.id = @ticket_id
            FOR UPDATE OF u, t                                      
            """;

        CommandDefinition command = new(
            sql,
            new { user_id = @event.UserId, ticket_id = @event.TicketId },
            cancellationToken: ct,
            transaction: transaction
        );

        int? exists = await connection.QueryFirstOrDefaultAsync<int?>(command);
        return exists is null
            ? Status.NotFound("Не найдена заявка по пользователю.")
            : Status.Success();
    }

    private async Task DeleteUserTicket(
        Domain.Users.Events.UserConfirmedEmail @event,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default
    )
    {
        const string sql = "DELETE FROM users_module.tickets WHERE id = @id";

        CommandDefinition command = new(
            sql,
            new { id = @event.TicketId },
            cancellationToken: ct,
            transaction: transaction
        );

        await connection.ExecuteAsync(command);
    }

    private async Task UpdateUserEmailConfirmation(
        Domain.Users.Events.UserConfirmedEmail @event,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct = default
    )
    {
        const string sql =
            "UPDATE users_module.users SET email_confirmed = @confirmed WHERE id = @id";

        CommandDefinition command = new(
            sql,
            new { confirmed = true, id = @event.UserId },
            cancellationToken: ct,
            transaction: transaction
        );

        await connection.ExecuteAsync(command);
    }
}
