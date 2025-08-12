using Npgsql;
using Users.Module.CommonAbstractions;
using Users.Module.Features.RegisteringUser.Exceptions;

namespace Users.Module.Features.RegisteringUser.Storage;

internal sealed class PgNewUsersStorage(PgConnectionSource connectionSource) : INewUsersStorage
{
    public void Dispose() => connectionSource.Dispose();

    public ValueTask DisposeAsync() => connectionSource.DisposeAsync();

    private const string SaveSql = """
        INSERT INTO users_module.users(id, name, password, email, email_confirmed)
        VALUES(@id, @name, @password, @email, @email_confirmed);
        """;
    private const string EmailParam = "@email";
    private const string NameParam = "@name";
    private const string IdParam = "@id";
    private const string PasswordParam = "@password";
    private const string EmailConfirmedParam = "@email_confirmed";

    public async Task<bool> Save(
        string name,
        string email,
        string password,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, name));
        command.Parameters.Add(new NpgsqlParameter<string>(EmailParam, email));
        command.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, Guid.NewGuid()));
        command.Parameters.Add(new NpgsqlParameter<string>(PasswordParam, password));
        command.Parameters.Add(new NpgsqlParameter<bool>(EmailConfirmedParam, false));
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
