using Npgsql;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal interface IExistingUsersStorage
{
    Task<IExistingUser> Get(
        IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand> method,
        CancellationToken ct = default
    );
}
