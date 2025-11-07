using Dapper;
using System.Data;
using Mailing.Module.Domain.Models;

namespace Mailing.Module.Infrastructure.NpgSql.Models;

// CREATE TABLE mailing_module.postmans
// (
//     id            UUID PRIMARY KEY,
//     email         varchar(255) not null UNIQUE,
//     password      varchar(512) not null,
//     current_sent  INT          NOT NULL,
//     current_limit INT          NOT NULL
// );
internal sealed class PgMailer
{
    private readonly IDbConnection _connection;
    private readonly PgMetadata _metadata;
    private readonly PgStatistics _statistics;
    private readonly DynamicParameters _parameters;

    public PgMailer(IDbConnection connection, IMailer mailer)
    {
        _connection = connection;
        PgMetadata metadata = new(mailer);
        PgStatistics statistics = new(mailer);
        _metadata = metadata;
        _statistics = statistics;
        _parameters = metadata.WriteTo(_parameters);
        _parameters = statistics.WriteTo(_parameters);
    }

    public Task Delete(CancellationToken ct = default) =>
        _connection.ExecuteAsync(
            new CommandDefinition(
                """
                DELETE FROM mailing_module.postmans
                WHERE id = @id
                """,
                _parameters,
                cancellationToken: ct));

    public Task Update(CancellationToken ct = default) =>
        _connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE mailing_module.postmans
            SET email = @email,
                password = @password,
                current_sent = @current_sent,
                current_limit = @current_limit
                WHERE id = @id
            """,
            _parameters,
            cancellationToken: ct));

    public Task Save(CancellationToken ct) =>
        _connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO mailing_module.postmans
            (id, email, password, current_sent, current_limit)
            VALUES
            (@id, @email, @password, @current_sent, @current_limit)
            """,
            _parameters,
            cancellationToken: ct));

    public Task<bool> HasUniqueEmail(CancellationToken ct = default) =>
        _connection.QuerySingleAsync<bool>(new CommandDefinition(
            """
            SELECT EXISTS(SELECT 1 FROM mailing_module.postmans
            WHERE email = @email
            """,
            _parameters,
            cancellationToken: ct));
}