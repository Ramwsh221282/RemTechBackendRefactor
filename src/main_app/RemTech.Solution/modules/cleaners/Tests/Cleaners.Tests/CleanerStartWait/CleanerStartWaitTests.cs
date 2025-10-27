using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.CleanerStartWait;

public sealed class CleanerStartWaitTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public CleanerStartWaitTests(CleanersTestHostFactory factory)
    {
        _sp = factory.Services;
    }

    [Fact]
    private async Task Not_Existing_Cleaner_Cannot_Start_Waiting()
    {
        var startWait = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(Guid.NewGuid()))
        );
        Assert.True(startWait.IsFailure);
    }

    [Fact]
    private async Task Waiting_Cleaner_Cannot_Start_Waiting()
    {
        var cleaner = await CreateCleaner();
        Assert.True(cleaner.IsSuccess);

        var result = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(result.IsSuccess);

        var startWait = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(result.Value.Id))
        );
        Assert.True(startWait.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, startWait.Value.State);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get(result.Value.Id)
        );
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, fromDb.Value.State);

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(result.Value.Id)
        );
        Assert.True(fromCache.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, fromCache.Value.State);

        var error = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(error.IsFailure);
    }

    [Fact]
    private async Task Working_Cleaner_Started_Waiting()
    {
        var cleaner = await CreateCleaner();
        Assert.True(cleaner.IsSuccess);

        var result = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(result.IsSuccess);

        var startWait = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(result.Value.Id))
        );
        Assert.True(startWait.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, startWait.Value.State);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get(result.Value.Id)
        );
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, fromDb.Value.State);

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(result.Value.Id)
        );
        Assert.True(fromCache.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, fromCache.Value.State);
    }

    [Fact]
    private async Task Disabled_Cleaner_Started_Waiting()
    {
        var cleaner = await CreateCleaner();
        Assert.True(cleaner.IsSuccess);

        var command = new StartWaitCommand(cleaner.Value.Id);
        await using var scope = _sp.CreateAsyncScope();
        var result = await scope
            .GetService<
                ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
            >()
            .Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, result.Value.State);

        await using var scope2 = _sp.CreateAsyncScope();
        var fromDb = await scope2.GetService<ICleanersStorage>().Get();

        Assert.True(fromDb.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, fromDb.Value.State);

        await using var scope3 = _sp.CreateAsyncScope();
        var fromCache = await scope3.GetService<ICleanersCachedStorage>().Get(result.Value.Id);

        Assert.True(fromCache.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.WaitingState, result.Value.State);
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
