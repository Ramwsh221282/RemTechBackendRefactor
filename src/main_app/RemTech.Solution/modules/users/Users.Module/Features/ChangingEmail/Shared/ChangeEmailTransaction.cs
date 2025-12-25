using Npgsql;
using Shared.Infrastructure.Module.Postgres;

namespace Users.Module.Features.ChangingEmail.Shared;

internal sealed class ChangeEmailTransaction(PostgresDatabase dataSource)
{
    private const string Sql = """
        UPDATE users_module.users SET email = @email, email_confirmed = FALSE WHERE id = @id
        """;

    public async Task Execute(string newEmail, Guid userId, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.DataSource.OpenConnectionAsync(
            ct
        );

        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();

        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@email", newEmail));
        sqlCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", userId));

        try
        {
            await sqlCommand.ExecuteNonQueryAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (NpgsqlException ex)
        {
            await transaction.RollbackAsync(ct);

            if (ex.Message.Contains("email"))
                throw new EmailDuplicateException();
            throw;
        }
    }
}
