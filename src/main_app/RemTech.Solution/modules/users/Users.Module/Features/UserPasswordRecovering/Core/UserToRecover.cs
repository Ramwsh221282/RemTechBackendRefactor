using System.Data.Common;
using Mailing.Module.Bus;
using Npgsql;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.Features.UserPasswordRecovering.Exceptions;
using Users.Module.Features.UserPasswordRecovering.Infrastructure;

namespace Users.Module.Features.UserPasswordRecovering.Core;

internal sealed class UserToRecover
{
    private readonly Guid _id;
    private readonly string _email;
    private readonly bool _emailConfirmed;

    private UserToRecover(Guid id, string email, bool emailConfirmed)
    {
        _id = id;
        _email = email;
        _emailConfirmed = emailConfirmed;
    }

    public static async Task<UserToRecover> Create(
        NpgsqlCommand command,
        CancellationToken ct = default
    )
    {
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UserToRecoverNotFoundException();
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string email = reader.GetString(reader.GetOrdinal("email"));
        bool emailConfirmed = reader.GetBoolean(reader.GetOrdinal("email_confirmed"));
        return new UserToRecover(id, email, emailConfirmed);
    }

    public IUserRecoveringMethod DecideMethod(
        ConnectionMultiplexer multiplexer,
        FrontendUrl frontendUrl,
        MailingBusPublisher publisher
    )
    {
        return _emailConfirmed
            ? new WhenEmailConfirmedRecoveringMethod(multiplexer, this, frontendUrl, publisher)
            : new WhenEmailNotConfirmedRecoveringMethod(multiplexer, this, frontendUrl, publisher);
    }

    public void Print(out Guid id, out string email, out bool emailConfirmed)
    {
        id = _id;
        email = _email;
        emailConfirmed = _emailConfirmed;
    }
}
