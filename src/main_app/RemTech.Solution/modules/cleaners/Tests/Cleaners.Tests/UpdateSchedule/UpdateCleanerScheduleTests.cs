using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Cleaners.Domain.Cleaners.UseCases.UpdateSchedule;
using Cleaners.WebApi.Extensions;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Cleaners.Tests.UpdateSchedule;

public sealed class UpdateCleanerScheduleTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public UpdateCleanerScheduleTests(CleanersTestHostFactory factory)
    {
        _sp = factory.Services;
    }

    [Fact]
    private async Task Cleaner_updates_schedule_wait_days_success()
    {
        int nextWaitDays = 3;

        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var withUpdatedSchedule = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        UpdateScheduleCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler => handler.Handle(new UpdateScheduleCommand(cleaner.Value.Id, nextWaitDays))
        );
        Assert.True(withUpdatedSchedule.IsSuccess);

        var fromDb = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );
        Assert.True(fromDb.IsSuccess);

        var fromCache = await _sp.ScopedExecution(
            scope => scope.GetService<ICleanersStorage>(),
            storage => storage.Get(cleaner.Value.Id)
        );
        Assert.True(fromDb.IsSuccess);

        var dbDto = fromDb.ToDto();
        var cacheDto = fromCache.ToDto();

        Assert.Equal(nextWaitDays, dbDto.WaitDays);
        Assert.Equal(nextWaitDays, cacheDto.WaitDays);
    }

    [Fact]
    private async Task Cleaner_cannot_make_wait_days_more_seven_days()
    {
        int nextWaitDays = 8;

        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var withUpdatedSchedule = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        UpdateScheduleCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler => handler.Handle(new UpdateScheduleCommand(cleaner.Value.Id, nextWaitDays))
        );
        Assert.True(withUpdatedSchedule.IsFailure);
    }

    [Fact]
    private async Task Cleaner_cannot_make_wait_days_less_one_day()
    {
        int nextWaitDays = 0;

        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var withUpdatedSchedule = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        UpdateScheduleCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler => handler.Handle(new UpdateScheduleCommand(cleaner.Value.Id, nextWaitDays))
        );
        Assert.True(withUpdatedSchedule.IsFailure);
    }

    [Fact]
    private async Task Cleaner_cannot_change_wait_days_if_working()
    {
        int nextWaitDays = 3;

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

        var withUpdatedSchedule = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        UpdateScheduleCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler => handler.Handle(new UpdateScheduleCommand(cleaner.Value.Id, nextWaitDays))
        );
        Assert.True(withUpdatedSchedule.IsFailure);
    }
}
