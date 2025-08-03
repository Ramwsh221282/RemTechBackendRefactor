using System.Data.Common;
using Npgsql;
using Users.Module.CommonAbstractions;
using Users.Module.Features.AuthenticateUser.Exceptions;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class PgExistingUsersStorage(PgConnectionSource connectionSource)
    : IExistingUsersStorage
{
    public async Task<IExistingUser> Get(
        IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand> method,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT id, name, password, email, email_confirmed
            FROM users_module.users
            """
        );
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        NpgsqlCommand modified = method.ModifyQuery(command);
        if (command.Parameters.Count == 0)
            throw new UnableToDetermineUserGetMethodException();
        await using DbDataReader reader = await modified.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UserDoesNotExistsException();
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        string password = reader.GetString(reader.GetOrdinal("password"));
        string email = reader.GetString(reader.GetOrdinal("email"));
        bool emailConfirmed = reader.GetBoolean(reader.GetOrdinal("email_confirmed"));
        return new ExistingUser(id, name, email, password, emailConfirmed);
    }
}
