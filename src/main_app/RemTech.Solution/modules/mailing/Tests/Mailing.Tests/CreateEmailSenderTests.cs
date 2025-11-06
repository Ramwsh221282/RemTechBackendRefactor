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
}