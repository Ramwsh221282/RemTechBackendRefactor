using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.AdjustWorkTime;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Cleaners.Domain.Cleaners.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.CleanerWorkTimeChanged;

public sealed class CleanerWorkTimeChangedTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public CleanerWorkTimeChangedTests(CleanersTestHostFactory factory) => _sp = factory.Services;

    [Fact]
    private async Task Working_cleaner_changed_work_time()
    {
        var cleaner = await _sp.ScopedExecution(
            scope => scope.GetService<ICommandHandler<CreateCleanerCommand, Status<Cleaner>>>(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var working = await _sp.ScopedExecution(
            scope => scope.GetService<ICommandHandler<StartWorkCommand, Status<Cleaner>>>(),
            handler => handler.Handle(new StartWorkCommand(cleaner.Value.Id))
        );
        Assert.True(working.IsSuccess);

        long workTime = 150000;
        var withChangedTime = await _sp.ScopedExecution(
            scope => scope.GetService<ICommandHandler<AdjustWorkTimeCommand, Status<Cleaner>>>(),
            handler => handler.Handle(new AdjustWorkTimeCommand(cleaner.Value.Id, workTime))
        );
        Assert.True(withChangedTime.IsSuccess);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersCachedStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );

        var expectedWorkTime = CleanerWorkTime.Create(workTime);

        Assert.True(expectedWorkTime.IsSuccess);
        Assert.True(fromDb.IsSuccess);
        Assert.True(fromCache.IsSuccess);
        Assert.Equal(expectedWorkTime.Value, fromDb.Value.WorkTime);
        Assert.Equal(expectedWorkTime.Value, fromCache.Value.WorkTime);
    }

    [Fact]
    private async Task Not_working_cleaner_cannot_change_work_time()
    {
        var cleaner = await _sp.ScopedExecution(
            scope => scope.GetService<ICommandHandler<CreateCleanerCommand, Status<Cleaner>>>(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        long workTime = 150000;
        var withChangedTime = await _sp.ScopedExecution(
            scope => scope.GetService<ICommandHandler<AdjustWorkTimeCommand, Status<Cleaner>>>(),
            handler => handler.Handle(new AdjustWorkTimeCommand(cleaner.Value.Id, workTime))
        );
        Assert.True(withChangedTime.IsFailure);
    }

    [Fact]
    private async Task Not_existing_cleaner_cannot_change_work_time()
    {
        long workTime = 150000;
        var withChangedTime = await _sp.ScopedExecution(
            scope => scope.GetService<ICommandHandler<AdjustWorkTimeCommand, Status<Cleaner>>>(),
            handler => handler.Handle(new AdjustWorkTimeCommand(Guid.NewGuid(), workTime))
        );
        Assert.True(withChangedTime.IsFailure);
    }

    [Fact]
    private async Task Negative_work_time_cannot_be_applied()
    {
        long workTime = -150000;
        var withChangedTime = await _sp.ScopedExecution(
            scope => scope.GetService<ICommandHandler<AdjustWorkTimeCommand, Status<Cleaner>>>(),
            handler => handler.Handle(new AdjustWorkTimeCommand(Guid.NewGuid(), workTime))
        );
        Assert.True(withChangedTime.IsFailure);
    }
}
