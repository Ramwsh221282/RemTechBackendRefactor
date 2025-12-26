using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.Disable;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.DisableCleaner;

public sealed class DisableCleanerTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public DisableCleanerTests(CleanersTestHostFactory factory)
    {
        _sp = factory.Services;
    }

    [Fact]
    private async Task Waiting_Cleaner_Disabled()
    {
        var cleaner = await CreateCleaner();
        var result = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(result.IsSuccess);

        var disabling = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<DisableCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(result.Value.Id))
        );
        Assert.True(disabling.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.DisabledState, disabling.Value.State);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get()
        );
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.DisabledState, fromDb.Value.State);

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );
        Assert.True(fromCache.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.DisabledState, fromCache.Value.State);
    }

    [Fact]
    private async Task Working_Cleaner_Disabled()
    {
        var cleaner = await CreateCleaner();
        var result = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(result.IsSuccess);

        var working = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(working.IsSuccess);

        var disabling = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<DisableCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(result.Value.Id))
        );
        Assert.True(disabling.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.DisabledState, disabling.Value.State);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get()
        );
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.DisabledState, fromDb.Value.State);

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );
        Assert.True(fromCache.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.DisabledState, fromCache.Value.State);
    }

    [Fact]
    private async Task Disabled_Cleaner_Was_Not_Disabled()
    {
        var cleaner = await CreateCleaner();
        var result = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(result.IsSuccess);

        var working = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(working.IsSuccess);

        var disabling = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<DisableCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(result.Value.Id))
        );
        Assert.True(disabling.IsSuccess);
        Assert.Equal(Domain.Cleaners.Aggregate.Cleaner.DisabledState, disabling.Value.State);

        var disablingAgain = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<DisableCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(result.Value.Id))
        );
        Assert.True(disablingAgain.IsFailure);
    }

    [Fact]
    private async Task Not_Existing_Cleaner_Cannot_be_Disabled()
    {
        var disabling = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<DisableCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new(Guid.NewGuid()))
        );
        Assert.True(disabling.IsFailure);
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
