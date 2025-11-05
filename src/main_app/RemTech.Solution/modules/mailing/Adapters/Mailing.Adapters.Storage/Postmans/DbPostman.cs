using System.Data;
using Dapper;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Adapters.Storage.Postmans;

/* CREATE TABLE mailing_module.postmans
// (
//     id            UUID PRIMARY KEY,
//     email         varchar(255) not null UNIQUE,
//     password      varchar(512) not null,
//     current_sent  INT          NOT NULL,
//     current_limit INT          NOT NULL
 ) */

internal sealed class DbPostman(PostgresDatabase database, DynamicParameters parameters, CancellationToken ct)
    : IDisposable
{
    private IDbConnection? _connection;

    public async Task Delete()
    {
        _connection ??= await database.ProvideConnection();
        await _connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM mailing_module.postmans 
            WHERE id = @id
            """,
            parameters,
            cancellationToken: ct));
    }

    public async Task Update()
    {
        _connection ??= await database.ProvideConnection();
        await _connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE mailing_module.postmans
            SET email = @email,
                password = @password
                WHERE id = @id
            """,
            parameters,
            cancellationToken: ct
        ));
    }

    public async Task Save()
    {
        _connection ??= await database.ProvideConnection();
        await _connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO mailing_module.postmans
            (id, email, password)
            VALUES
            (@id, @email, @password)
            """,
            parameters,
            cancellationToken: ct));
    }

    public async Task<bool> HasUniqueEmail()
    {
        _connection ??= await database.ProvideConnection();
        return !(await _connection.QuerySingleAsync<bool>(
            new CommandDefinition(
                """
                SELECT 
                    EXISTS(SELECT 1 FROM mailing_module.postmans 
                WHERE 
                    email = @email);
                """,
                parameters,
                cancellationToken: ct)));
    }

    public void Dispose() => _connection?.Dispose();
}