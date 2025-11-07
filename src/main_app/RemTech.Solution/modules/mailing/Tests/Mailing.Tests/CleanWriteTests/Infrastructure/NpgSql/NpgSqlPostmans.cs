using System.Data;
using Dapper;
using Mailing.Tests.CleanWriteTests.Models;
using RemTech.Core.Shared.Async;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class NpgSqlPostmans(PostgresDatabase database) : IPostmans
{
    public Future Save(ITestPostman postman, CancellationToken ct = default) =>
        NpgSqlPostman.FromPostman(database, postman).Save(ct);

    public Future Remove(ITestPostman postman, CancellationToken ct = default) =>
        NpgSqlPostman.FromPostman(database, postman).Delete(ct);

    public FromFuture<ITestPostman> FindById(Guid id, CancellationToken ct = default) =>
        new(async () =>
        {
            using IDbConnection connection = await database.ProvideConnection(ct: ct);
            TablePostman? postman = await connection.QueryFirstOrDefaultAsync<TablePostman>(
                new CommandDefinition(
                    "SELECT * FROM mailing_module.postmans WHERE id=@id))",
                    new { @id = id },
                    cancellationToken: ct));
            return postman is null ? new EmptyPostman() : postman.ToPostman();
        });

    public FromFuture<ITestPostman> FindByEmail(string email, CancellationToken ct = default) =>
        new FromFuture<ITestPostman>(async () =>
        {
            using IDbConnection connection = await database.ProvideConnection(ct: ct);
            TablePostman? postman = await connection.QueryFirstOrDefaultAsync<TablePostman>(
                new CommandDefinition(
                    "SELECT * FROM mailing_module.postmans WHERE email=@email))",
                    new { @email = email },
                    cancellationToken: ct));
            return postman is null ? new EmptyPostman() : postman.ToPostman();
        });
}