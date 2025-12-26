using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.StartWork;

public sealed class StartWorkTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public StartWorkTests(CleanersTestHostFactory factory)
    {
        _sp = factory.Services;
    }

    [Fact]
    private async Task Disabled_cleaner_starts_work()
    {
        var cleaner = await CreateCleaner();
        Assert.True(cleaner.IsSuccess);

        var command = new StartWorkCommand(cleaner.Value.Id);
        await using var scope = _sp.CreateAsyncScope();
        var result = await scope
            .GetService<
                ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);
        Assert.True(result.IsSuccess);

        await using var scope2 = _sp.CreateAsyncScope();
        var fromDb = await scope2.GetService<ICleanersStorage>().Get();
        Assert.True(fromDb.IsSuccess);

        await using var scope3 = _sp.CreateAsyncScope();
        var fromCache = await scope3.GetService<ICleanersCachedStorage>().Get(result.Value.Id);
        Assert.True(fromCache.IsSuccess);

        Assert.True(result.Value.State == Domain.Cleaners.Aggregate.Cleaner.WorkState);
        Assert.True(fromDb.Value.State == Domain.Cleaners.Aggregate.Cleaner.WorkState);
        Assert.True(fromCache.Value.State == Domain.Cleaners.Aggregate.Cleaner.WorkState);
    }

    [Fact]
    private async Task Not_existed_cleaner_does_not_start_work()
    {
        var command = new StartWorkCommand(Guid.NewGuid());
        await using var scope = _sp.CreateAsyncScope();
        var result1 = await scope
            .GetService<
                ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);
        Assert.True(result1.IsFailure);
    }

    [Fact]
    private async Task Started_cleaner_cannot_start_work()
    {
        var cleaner = await CreateCleaner();
        Assert.True(cleaner.IsSuccess);

        var command = new StartWorkCommand(cleaner.Value.Id);
        await using var scope = _sp.CreateAsyncScope();
        var result1 = await scope
            .GetService<
                ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);
        Assert.True(result1.IsSuccess);

        await using var scope2 = _sp.CreateAsyncScope();
        var result2 = await scope2
            .GetService<
                ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);
        Assert.True(result2.IsFailure);
    }

    private async Task<Status<Domain.Cleaners.Aggregate.Cleaner>> CreateCleaner()
    {
        var command = new CreateCleanerCommand();
        await using var scope = _sp.CreateAsyncScope();

        var cleaner = await scope
            .GetService<
                ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);

        return cleaner;
    }
}
