using Npgsql;
using Users.Module.CommonAbstractions;
using Users.Module.Features.RegisteringUser.Exceptions;

namespace Users.Module.Features.RegisteringUser.Storage;

internal sealed class PgUsersStorage(PgConnectionSource connectionSource) : IUsersStorage
{
    public void Dispose() => connectionSource.Dispose();

    public ValueTask DisposeAsync() => connectionSource.DisposeAsync();

    public async Task<bool> Save(
        string name,
        string email,
        string password,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        string sql = string.Intern(
            """
            INSERT INTO users_module.users(id, name, password, email, email_confirmed)
            VALUES(@id, @name, @password, @email, @email_confirmed);
            """
        );
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
        command.Parameters.Add(new NpgsqlParameter<string>("@email", email));
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", Guid.NewGuid()));
        command.Parameters.Add(new NpgsqlParameter<string>("@password", password));
        command.Parameters.Add(new NpgsqlParameter<bool>("@email_confirmed", false));
        try
        {
            await command.ExecuteNonQueryAsync(ct);
        }
        catch (PostgresException ex)
        {
            string? constraint = ex.ConstraintName;
            switch (constraint)
            {
                case "users_email_key":
                    throw new UserRegistrationEmailConflictException(email);
                case "users_name_key":
                    throw new UserRegistrationNickNameConflictException(name);
                default:
                    throw;
            }
        }

        return true;
    }
}
