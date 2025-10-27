using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.CleanItem;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.CleanItem;

public sealed class CleanItemTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public CleanItemTests(CleanersTestHostFactory factory)
    {
        _sp = factory.Services;
    }

    [Fact]
    private async Task Waiting_cleaner_cannot_clean_item()
    {
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
            handler => handler.Handle(new(cleaner.Value.Id))
        );
        Assert.True(waiting.IsSuccess);

        var itemCleaned = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CleanItemCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CleanItemCommand(cleaner.Value.Id))
        );
        Assert.True(itemCleaned.IsFailure);
    }

    [Fact]
    private async Task Disabled_cleaner_cannot_clean_item()
    {
        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var itemCleaned = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CleanItemCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CleanItemCommand(cleaner.Value.Id))
        );
        Assert.True(itemCleaned.IsFailure);
    }

    [Fact]
    private async Task Working_Cleaner_cleaned_item()
    {
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

        var itemCleaned = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CleanItemCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CleanItemCommand(cleaner.Value.Id))
        );
        Assert.True(itemCleaned.IsSuccess);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get()
        );
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(1, fromDb.Value.CleanedAmount);

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(1, fromCache.Value.CleanedAmount);
    }

    [Fact]
    private async Task Not_Existing_Cleaner_cannot_clean_item()
    {
        var result = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CleanItemCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CleanItemCommand(Guid.Empty))
        );
        Assert.True(result.IsFailure);
    }
}
