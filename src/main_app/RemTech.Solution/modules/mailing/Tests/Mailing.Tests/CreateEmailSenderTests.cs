using Mailing.Tests.CleanWriteTests.Domain.Implementations;
using Mailing.Tests.CleanWriteTests.Infrastructure;
using Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;
using Mailing.Tests.CleanWriteTests.Infrastructure.Redis;
using Mailing.Tests.CleanWriteTests.Interactor.Implementation;
using Mailing.Tests.CleanWriteTests.Models;
using Mailing.Tests.CleanWriteTests.Presenter;
using RemTech.Core.Shared.Primitives.Async;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;

namespace Mailing.Tests;

public sealed class CreateEmailSenderTests(MailingTestServices services) : IClassFixture<MailingTestServices>
{
    [Fact]
    private async Task Create_sender_OOP_success()
    {
    }

    private async Task<Status<Unit>> Test(
        PostmanDto dto,
        DelayedUnitStatus container,
        PostgresDatabase db,
        RedisCache cache,
        CancellationToken ct)
    {
        var meta = new TestPostmanMetadata(Guid.NewGuid(), "test@mail.com", "test-password");
        var stats = new TestPostmanStatistics(0, 0);
        var postman = new TestPostman(meta, stats);
        var writePg = new WritePostmanInPostgres(db, ct, container);
        var writeRedis = new WritePostmanInRedis(cache, container);
        var writePresenter = new WritePostmanInDto(dto);
        var writeComposer = new WritePostmanInfrastructureComposer().Add(writePg).Add(writeRedis);
        var writeInteractive = new WritePostmanInteractive(writeComposer, writePresenter);
        var writeByDomain = new WritePostmanByDomain(writeInteractive);
        postman.Write(writeByDomain);
        return await container.Read();
    }
}

public interface IComponent<in TIn, out TOut>
{
    TOut Execute(TIn arg);
}

public sealed class
    WritePostmanComponent(PostgresDatabase db, RedisCache cache, DelayedUnitStatus container)
    : IComponent<Parameters<PostmanDto, CancellationToken>,
        Task<Status<Unit>>>
{
    public async Task<Status<Unit>> Execute(Parameters<PostmanDto, CancellationToken> arg)
    {
        var meta = new TestPostmanMetadata(arg.First.Id, arg.First.Email, arg.First.Password);
        var stats = new TestPostmanStatistics(0, 0);
        var postman = new TestPostman(meta, stats);
        var writePg = new WritePostmanInPostgres(db, arg.Second, container);
        var writeRedis = new WritePostmanInRedis(cache, container);
        var writePresenter = new WritePostmanInDto(arg.First);
        var writeComposer = new WritePostmanInfrastructureComposer().Add(writePg).Add(writeRedis);
        var writeInteractive = new WritePostmanInteractive(writeComposer, writePresenter);
        var writeByDomain = new WritePostmanByDomain(writeInteractive);
        postman.Write(writeByDomain);
        return await container.Read();
    }
}

public sealed class Future
{
    private readonly TaskCompletionSource<Status<Unit>> _tcs;

    public void Complete(Status<Unit> result) => _tcs.TrySetResult(result);
}

public sealed class Parameters<T1>(T1 first)
{
    public T1 First => first;
}

public sealed class Parameters<T1, T2>(T1 first, T2 second)
{
    public T1 First => first;
    public T2 Second => second;
}

public sealed class Parameters<T1, T2, T3>(T1 first, T2 second, T3 third)
{
    public T1 First => first;
    public T2 Second => second;
    public T3 Third => third;
}