using Mailing.Tests.CleanWriteTests.Domain.Implementations;
using Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;
using Mailing.Tests.CleanWriteTests.Interactor.Implementation;
using Mailing.Tests.CleanWriteTests.Models;
using Mailing.Tests.CleanWriteTests.Presenter;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Tests;

public sealed class CreateEmailSenderTests(MailingTestServices services) : IClassFixture<MailingTestServices>
{
    [Fact]
    private async Task Create_sender_OOP_success()
    {
        await using var scope = services.Scope();
        var db = scope.GetService<PostgresDatabase>();

        CancellationToken ct = CancellationToken.None;
        PostmanDto dto = new PostmanDto();
        TaskCompletionSource<Status<Unit>> tcs = new();

        TestPostmanMetadata meta = new TestPostmanMetadata(Guid.NewGuid(), "test@mail.com", "test-password");
        TestPostmanStatistics stats = new(0, 0);
        TestPostman postman = new(meta, stats);

        WritePostmanInPostgres npgPostmanInPostgres = new(db, ct, tcs);
        WritePostmanInDto presenterPostmanInDto = new(dto);
        WritePostmanInteractive interactorPostmanInteractive = new(npgPostmanInPostgres, presenterPostmanInDto);
        WritePostmanByDomain byDomainPostmanBy = new(interactorPostmanInteractive);
        postman.Write(byDomainPostmanBy);
        Status<Unit> result = await tcs.Task;
        Assert.True(result.IsSuccess);
    }

    private async Task Test(PostmanDto dto, TaskCompletionSource<Status<Unit>> tcs, PostgresDatabase db,
        CancellationToken ct)
    {
        // todo add cache.
        var meta = new TestPostmanMetadata(Guid.NewGuid(), "test@mail.com", "test-password");
        var stats = new TestPostmanStatistics(0, 0);
        var postman = new TestPostman(meta, stats);
        var writePg = new WritePostmanInPostgres(db, ct, tcs);
        var writePresenter = new WritePostmanInDto(dto);
        var writeInteractive = new WritePostmanInteractive(writePg, writePresenter);
        var writeByDomain = new WritePostmanByDomain(writeInteractive);
        postman.Write(writeByDomain);
        var result = await tcs.Task;
    }
}