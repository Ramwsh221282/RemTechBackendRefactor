using Dapper;
using Mailing.Tests.CleanWriteTests.Models;
using RemTech.Core.Shared.Async;
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
public sealed class NpgSqlPostman(SqlInteractor interactor, DynamicParameters parameters) : IDisposable
{
    internal static NpgSqlPostman FromPostman(PostgresDatabase db, ITestPostman postman)
    {
        DynamicParameters parameters = new DynamicParameters();
        NpgSqlPostmanMetadata metadata = NpgSqlPostmanMetadata.FromPostman(postman);
        NpgSqlPostmanStatistics stats = NpgSqlPostmanStatistics.FromPostman(postman);
        metadata.WriteTo(parameters);
        stats.WriteTo(parameters);
        SqlInteractor interactor = new SqlInteractor(() => db.ProvideConnection());
        return postman.Transform(_ => new NpgSqlPostman(interactor, parameters));
    }

    public Future Delete(CancellationToken ct = default) =>
        new Future(() => interactor.ExecutePermanent(new CommandDefinition(
            """
            DELETE FROM mailing_module.postmans
            WHERE id = @id
            """,
            parameters,
            cancellationToken: ct)));

    public Future Update(CancellationToken ct = default) =>
        new Future(() => interactor.ExecutePermanent(new CommandDefinition(
            """
            UPDATE mailing_module.postmans
            SET email = @email,
                password = @password,
                current_sent = @current_sent,
                current_limit = @current_limit
                WHERE id = @id
            """,
            parameters,
            cancellationToken: ct)));

    public Future Save(CancellationToken ct) =>
        new(() => interactor.ExecutePermanent(new CommandDefinition(
            """
            INSERT INTO mailing_module.postmans
            (id, email, password, current_sent, current_limit)
            VALUES
            (@id, @email, @password, @current_sent, @current_limit)
            """,
            parameters,
            cancellationToken: ct)));

    public FromFuture<bool> HasUniqueEmail(CancellationToken ct = default) =>
        new(new FromFuture<bool>(interactor.ExecutePermanent((conn) =>
            conn.QuerySingleAsync<bool>(
                new CommandDefinition(
                    """
                    SELECT EXISTS(SELECT 1 FROM mailing_module.postmans
                    WHERE email = @email
                    """,
                    parameters,
                    cancellationToken: ct)))));


    public void Dispose() => interactor.Dispose();
}