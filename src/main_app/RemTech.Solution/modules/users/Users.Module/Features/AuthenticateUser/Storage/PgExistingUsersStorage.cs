using System.Data.Common;
using Npgsql;
using Users.Module.CommonAbstractions;
using Users.Module.Features.AuthenticateUser.Exceptions;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class PgExistingUsersStorage(PgConnectionSource connectionSource)
    : IExistingUsersStorage
{
    private const string Sql = """
        SELECT id, name, password, email, email_confirmed
        FROM users_module.users
        """;
    private const string IdColumn = "id";
    private const string NameColumn = "name";
    private const string PasswordColumn = "password";
    private const string EmailColumn = "email";
    private const string EmailConfirmedColumn = "email_confirmed";

    public async Task<IExistingUser> Get(
        IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand> method,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        NpgsqlCommand modified = method.ModifyQuery(command);
        if (command.Parameters.Count == 0)
            throw new UnableToDetermineUserGetMethodException();
        await using DbDataReader reader = await modified.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UserDoesNotExistsException();
        Guid id = reader.GetGuid(reader.GetOrdinal(IdColumn));
        string name = reader.GetString(reader.GetOrdinal(NameColumn));
        string password = reader.GetString(reader.GetOrdinal(PasswordColumn));
        string email = reader.GetString(reader.GetOrdinal(EmailColumn));
        bool emailConfirmed = reader.GetBoolean(reader.GetOrdinal(EmailConfirmedColumn));
        return new ExistingUser(id, name, email, password, emailConfirmed);
    }
}
