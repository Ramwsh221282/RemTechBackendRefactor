using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Extensions;
using Identity.Domain.Users.Aggregate;
using Identity.Notifier.Port;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Serilog;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.EventHandlers.UserCreatedEmailConfirmationTicket;

public sealed class UserCreatedEmailConfirmationTicketHandler(
    ILogger logger,
    PostgresDatabase database,
    IUsersNotifier notifier
) : IDomainEventHandler<Domain.Users.Events.UserCreatedEmailConfirmTicket>
{
    private const string Context = nameof(UserCreatedEmailConfirmationTicketHandler);

    public async Task<Status> Handle(
        Domain.Users.Events.UserCreatedEmailConfirmTicket @event,
        CancellationToken ct = default
    )
    {
        using var connection = await database.ProvideConnection(ct);
        using var transaction = connection.ProvideTransaction();
        try
        {
            var user = await GetUser(@event, connection, transaction, ct);
            if (user.IsFailure)
                return Status.NotFound("Пользователь не найден.");

            await InsertTicket(@event, user, connection, transaction, ct);
            var notificationBody = CreateNotification(@event, user);
            var notification = await notifier.Notify(notificationBody, transaction, ct);
            if (!notification.IsSuccess)
                return Status.Conflict(notification.Error);

            transaction.Commit();
            return Status.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.Error("{Context}. Error: {Error}.", Context, ex);
            return Status.Internal("Не удалось создать заявку на подтверждение почты.");
        }
    }

    private async Task<Status<User>> GetUser(
        Domain.Users.Events.UserCreatedEmailConfirmTicket @event,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct
    )
    {
        Guid userId = @event.EventArgs.UserId;

        User? user = await connection.QueryUser(
            ["u.id = @id"],
            new { id = userId },
            transaction,
            true,
            ct
        );

        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }

    private async Task InsertTicket(
        Domain.Users.Events.UserCreatedEmailConfirmTicket @event,
        User user,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct
    )
    {
        const string sql = """
            INSERT INTO users_module.tickets
            (id, user_id, type, created, expired)
            VALUES
            (@id, @user_id, @type, @created, @expired)
            """;

        var eventArgs = @event.EventArgs;
        var command = new CommandDefinition(
            sql,
            new
            {
                id = eventArgs.TicketId,
                user_id = user.Id.Id,
                type = eventArgs.Type,
                created = eventArgs.Created,
                expired = eventArgs.Expired,
            },
            cancellationToken: ct,
            transaction: transaction
        );

        await connection.ExecuteAsync(command);
    }

    private UserNotification CreateNotification(
        Domain.Users.Events.UserCreatedEmailConfirmTicket @event,
        User user
    )
    {
        Guid ticketId = @event.EventArgs.TicketId;
        string subject = "Подтверждение почты RemTech агрегатор спецтехники.";
        string message = $"""
            Была подана заявка на подтверждение почты.
            Для подтверждения почты необходимо перейти по ссылке:
            <a href="FRONTEND_URL/TOKEN">Подтверждение почты</a>
            """;

        return new UserNotification(ticketId, user.Profile.Email.Email, subject, message);
    }
}
