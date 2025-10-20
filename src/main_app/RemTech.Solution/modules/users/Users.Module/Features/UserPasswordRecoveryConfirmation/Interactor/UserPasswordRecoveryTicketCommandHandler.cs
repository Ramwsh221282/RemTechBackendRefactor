using System.Data.Common;
using Mailing.Module.Bus;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Features.UserPasswordRecovering.Infrastructure;
using Users.Module.Features.UserPasswordRecoveryConfirmation.Core;
using Users.Module.Models;

namespace Users.Module.Features.UserPasswordRecoveryConfirmation.Interactor;

internal sealed class UserPasswordRecoveryTicketCommandHandler
    : ICommandHandler<UserPasswordRecoveryTicketCommand>
{
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly StringHash _hash;
    private readonly NpgsqlDataSource _dataSource;
    private readonly MailingBusPublisher _publisher;

    public UserPasswordRecoveryTicketCommandHandler(
        ConnectionMultiplexer multiplexer,
        StringHash hash,
        NpgsqlDataSource dataSource,
        MailingBusPublisher publisher
    )
    {
        _multiplexer = multiplexer;
        _hash = hash;
        _dataSource = dataSource;
        _publisher = publisher;
    }

    public async Task Handle(
        UserPasswordRecoveryTicketCommand command,
        CancellationToken ct = default
    )
    {
        UserPasswordRecoveryTicket ticket = UserPasswordRecoveryTicket.Create(command.Key);
        Guid userId = await ReceiverUserIdFromCache(ticket);
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            UserWithRecoveredPassword user = await ReceiveUserFromDatabase(connection, userId, ct);
            await user.ResetPassword(_hash, connection, _publisher, ct);
            await transaction.CommitAsync(ct);
            await TryRemoveUserSession(userId);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<Guid> ReceiverUserIdFromCache(UserPasswordRecoveryTicket ticket)
    {
        ticket.Print(out Guid id);
        PasswordRecoveringCache cache = new(_multiplexer);
        Guid userId = await cache.ReceiveUserId(id.ToString());
        return userId;
    }

    private static async Task<UserWithRecoveredPassword> ReceiveUserFromDatabase(
        NpgsqlConnection connection,
        Guid id,
        CancellationToken ct
    )
    {
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = """
            SELECT
            users.id as id,
            users.email as email
            FROM users_module.users as users
            WHERE users.id = @id;
            """;
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("id", id));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UserNotFoundException();
        Guid userId = reader.GetGuid(reader.GetOrdinal("id"));
        string email = reader.GetString(reader.GetOrdinal("email"));
        return new UserWithRecoveredPassword(userId, email);
    }

    private async Task TryRemoveUserSession(Guid id)
    {
        try
        {
            UserJwt jwt = new(id);
            jwt = await jwt.Provide(_multiplexer);
            await jwt.Deleted(_multiplexer);
        }
        catch
        {
            // ignored
        }
    }
}
