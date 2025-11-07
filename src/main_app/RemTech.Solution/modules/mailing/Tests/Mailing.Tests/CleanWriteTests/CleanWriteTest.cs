using System.Data;
using Dapper;
using Mailing.Domain.Postmans;
using Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;
using Mailing.Tests.CleanWriteTests.Models;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Async;
using Shared.Infrastructure.Module.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Tests.CleanWriteTests;

public sealed class CleanWriteTest(MailingTestServices services) : IClassFixture<MailingTestServices>
{
    [Fact]
    private async Task Test()
    {
        await using AsyncServiceScope scope = services.Scope();
        IPostmans postmans = scope.GetService<IPostmans>();
        TestPostman postman = new(new TestPostmanMetadata(Guid.NewGuid(), "postman@mail.com", "123"));
        Future save = postmans.Save(postman, CancellationToken.None);
        await save.Complete();
    }
}

public interface IPostmanCriteria
{
    FromFuture<ITestPostman> Find(CancellationToken ct = default);
}

internal interface INpgPostmanCriteria
{
    void AttachPostgres(PostgresDatabase database);
}

public sealed record PostmanByIdCriteria(Guid Id) : IPostmanCriteria, INpgPostmanCriteria
{
    private PostgresDatabase? _database;
    private const string Sql = "SELECT * FROM mailing_module.postmans WHERE id = @id;";

    public void AttachPostgres(PostgresDatabase database) =>
        _database = database;

    public FromFuture<ITestPostman> Find(CancellationToken ct = default) =>
        _database == null
            ? new FromFuture<ITestPostman>(() => Task.FromResult<ITestPostman>(new EmptyPostman()))
            : new FromFuture<ITestPostman>(() => Query(_database, Id, ct));

    private static async Task<ITestPostman> Query(PostgresDatabase db, Guid id, CancellationToken ct)
    {
        using IDbConnection conn = await db.ProvideConnection(ct: ct);
        TablePostman? postman = await conn.QueryFirstOrDefaultAsync<TablePostman>(
            new CommandDefinition(
                sql,
                new { @id = id },
                cancellationToken: ct
            )
        );
        return postman is null ? new EmptyPostman() : postman.ToPostman();
    }
}

public sealed class PostmanByEmailCriteria : IPostmanCriteria, INpgPostmanCriteria
{
    private PostgresDatabase? _database;
    private const string Sql = "SELECT * FROM mailing_module.postmans WHERE email = @email;";

    public FromFuture<ITestPostman> Find(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public void AttachPostgres(PostgresDatabase database) =>
        _database = database;

    private static async Task<ITestPostman> Query(PostgresDatabase db, string email, CancellationToken ct)
    {
        using IDbConnection conn = await db.ProvideConnection(ct: ct);
        TablePostman? postman = await conn.QueryFirstOrDefaultAsync<TablePostman>(
            new CommandDefinition(
                Sql,
                new { @email = email },
                cancellationToken: ct
            )
        );
        return postman is null ? new EmptyPostman() : postman.ToPostman();
    }
}