using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.ChangeItemsToClean;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.ChangeItemsToCleanThreshold;

public sealed class ChangeItemsToCleanThresholdTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public ChangeItemsToCleanThresholdTests(CleanersTestHostFactory factory)
    {
        _sp = factory.Services;
    }

    [Fact]
    private async Task Disabled_Cleaner_Changes_Items_To_Clean()
    {
        int newThreshold = 3;
        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var changingThreshold = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        ChangeItemsToCleanTresholdCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler =>
                handler.Handle(
                    new ChangeItemsToCleanTresholdCommand(cleaner.Value.Id, newThreshold)
                )
        );
        Assert.True(changingThreshold.IsSuccess);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );

        Assert.True(fromDb.IsSuccess);
        Assert.True(fromCache.IsSuccess);

        Assert.Equal(newThreshold, fromDb.Value.ItemsDateDayThreshold);
        Assert.Equal(newThreshold, fromCache.Value.ItemsDateDayThreshold);
    }

    [Fact]
    private async Task Waiting_Cleaner_Changes_Items_To_Clean()
    {
        int newThreshold = 3;
        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var waiting = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new StartWaitCommand(cleaner.Value.Id))
        );
        Assert.True(waiting.IsSuccess);

        var changingThreshold = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        ChangeItemsToCleanTresholdCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler =>
                handler.Handle(
                    new ChangeItemsToCleanTresholdCommand(cleaner.Value.Id, newThreshold)
                )
        );
        Assert.True(changingThreshold.IsSuccess);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );

        Assert.True(fromDb.IsSuccess);
        Assert.True(fromCache.IsSuccess);

        Assert.Equal(newThreshold, fromDb.Value.ItemsDateDayThreshold);
        Assert.Equal(newThreshold, fromCache.Value.ItemsDateDayThreshold);
    }

    [Fact]
    private async Task Working_Cleaner_Cannot_Change_Items_Threshold()
    {
        int newThreshold = 3;
        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var working = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new StartWorkCommand(cleaner.Value.Id))
        );
        Assert.True(working.IsSuccess);

        var changingThreshold = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        ChangeItemsToCleanTresholdCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler =>
                handler.Handle(
                    new ChangeItemsToCleanTresholdCommand(cleaner.Value.Id, newThreshold)
                )
        );
        Assert.True(changingThreshold.IsFailure);
    }

    [Fact]
    private async Task Not_Existing_Cleaner_Cannot_Change_Items_Threshold()
    {
        int newThreshold = 3;
        var changingThreshold = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        ChangeItemsToCleanTresholdCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler =>
                handler.Handle(new ChangeItemsToCleanTresholdCommand(Guid.NewGuid(), newThreshold))
        );
        Assert.True(changingThreshold.IsFailure);
    }
}
