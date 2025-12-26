using System.Data;
using Dapper;
using Identity.Adapter.Storage.Storages.Extensions;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Identity.Adapter.Storage.EventHandlers.UserCreatedPasswordResetTicket;

public sealed class UserCreatedPasswordResetTicketHandler(
    Serilog.ILogger logger,
    PostgresDatabase database
) : IDomainEventHandler<Domain.Users.Events.UserCreatedPasswordResetTicket>
{
    private const string Context = nameof(UserCreatedPasswordResetTicketHandler);

    public async Task<Status> Handle(
        Domain.Users.Events.UserCreatedPasswordResetTicket @event,
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Context}: handling. User id - {Id}. Ticket id - {TicketId}.",
            Context,
            @event.EventArgs.UserId,
            @event.EventArgs.TicketId
        );

        using var connection = await database.ProvideConnection(ct: ct);
        using var transaction = connection.ProvideTransaction();

        try
        {
            Status exists = await EnsureUserExists(@event, connection, transaction, ct);
            if (exists.IsFailure)
                return exists;

            await InsertUserTicket(@event, connection, transaction, ct);
            transaction.Commit();

            logger.Information(
                "{Context}. User {Id} created password reset ticket. Ticket id - {TicketId}.",
                Context,
                @event.EventArgs.UserId,
                @event.EventArgs.TicketId
            );

            return Status.Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            logger.Error("{Context}. Error: {Text}.", Context, ex);
            return Status.Internal("Ошибка при сохранении заявки сброса пароля.");
        }
    }

    private async Task<Status> EnsureUserExists(
        Domain.Users.Events.UserCreatedPasswordResetTicket @event,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct
    ) => await connection.BlockUserRow(@event.EventArgs.UserId, transaction, ct);

    private async Task InsertUserTicket(
        Domain.Users.Events.UserCreatedPasswordResetTicket @event,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken ct
    )
    {
        var eventArgs = @event.EventArgs;
        DynamicParameters parameters = new();
        parameters = parameters.TicketInsertParams(
            eventArgs.TicketId,
            eventArgs.UserId,
            eventArgs.Type,
            eventArgs.Created,
            eventArgs.Expired
        );
        await connection.InsertTicket(parameters, transaction, ct);
    }
}
