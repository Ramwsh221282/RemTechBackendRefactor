using System.Data;
using Dapper;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

// CREATE TABLE mailing_module.postmans
// (
//     id            UUID PRIMARY KEY,
//     email         varchar(255) not null UNIQUE,
//     password      varchar(512) not null,
//     current_sent  INT          NOT NULL,
//     current_limit INT          NOT NULL
// );
public sealed class NpgSqlPostman : IDisposable, IWritePostmanMetadataInfrastructureCommand,
    IWritePostmanStatisticsInfrastructureCommand
{
    private readonly Lazy<Task<IDbConnection>> _lazyConnection;
    private readonly DynamicParameters _parameters;
    private readonly CancellationToken _ct;
    private IDbConnection? _connection;

    public NpgSqlPostman(PostgresDatabase database, CancellationToken ct)
    {
        _ct = ct;
        _parameters = new();
        _lazyConnection = new(async () => _connection ??= await database.ProvideConnection(ct));
    }

    private Task<IDbConnection> Connection => _lazyConnection.Value;

    public async Task Delete() =>
        await (await Connection).ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM mailing_module.postmans 
            WHERE id = @id
            """,
            _parameters,
            cancellationToken: _ct));

    public async Task Update() =>
        await (await Connection).ExecuteAsync(new CommandDefinition(
            """
            UPDATE mailing_module.postmans
            SET email = @email,
                password = @password,
                current_sent = @current_sent,
                current_limit = @current_limit
                WHERE id = @id
            """,
            _parameters,
            cancellationToken: _ct
        ));

    public async Task Save() =>
        await (await Connection).ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO mailing_module.postmans
            (id, email, password, current_sent, current_limit)
            VALUES
            (@id, @email, @password, @current_sent, @current_limit)
            """,
            _parameters,
            cancellationToken: _ct));

    public async Task<bool> HasUniqueEmail() =>
        !(await (await Connection).QuerySingleAsync<bool>(
            new CommandDefinition(
                """
                SELECT 
                    EXISTS(SELECT 1 FROM mailing_module.postmans 
                WHERE 
                    email = @email);
                """,
                _parameters,
                cancellationToken: _ct)));

    public void Dispose()
    {
        if (!_lazyConnection.IsValueCreated) return;
        Task<IDbConnection> task = _lazyConnection.Value;
        if (task.IsCompletedSuccessfully)
            task.Result.Dispose();
    }

    public void Execute(in Guid id, in string email, in string password)
    {
        _parameters.Add("@id", id, DbType.Guid);
        _parameters.Add("@email", email, DbType.String);
        _parameters.Add("@password", password, DbType.String);
    }

    public void Execute(in int sendLimit, in int currentSend)
    {
        _parameters.Add("@current_limit", sendLimit, DbType.Int32);
        _parameters.Add("@current_sent", currentSend, DbType.Int32);
    }
}