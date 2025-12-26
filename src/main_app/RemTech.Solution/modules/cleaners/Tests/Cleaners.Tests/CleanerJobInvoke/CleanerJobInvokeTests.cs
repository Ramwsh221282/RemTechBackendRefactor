using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
using Cleaners.Domain.Cleaners.UseCases.UpdateSchedule;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaners.Tests.CleanerJobInvoke;

public sealed class CleanerJobInvokeTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public CleanerJobInvokeTests(CleanersTestHostFactory factory) => _sp = factory.Services;

    [Fact]
    private async Task Start_Cleaner_Ensure_Started()
    {
        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var updateWaitDays = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<
                        UpdateScheduleCommand,
                        Status<Domain.Cleaners.Aggregate.Cleaner>
                    >
                >(),
            handler => handler.Handle(new UpdateScheduleCommand(cleaner.Value.Id, 1))
        );
        Assert.True(updateWaitDays.IsSuccess);

        var startWaiting = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new StartWaitCommand(cleaner.Value.Id))
        );
        Assert.True(startWaiting.IsSuccess);

        await Force_Update_Cleaner(cleaner.Value.Id);
        bool hasStarted = await Ensure_Cleaner_Job_Started(cleaner.Value.Id);
        Assert.True(hasStarted);
    }

    private async Task Force_Update_Cleaner(Guid cleanerId)
    {
        var current = DateTime.UtcNow;
        const string sql =
            "UPDATE cleaners_module.cleaners SET next_run = @next_run WHERE id = @id";
        var command = new CommandDefinition(sql, new { next_run = current, id = cleanerId });
        await using var scope = _sp.CreateAsyncScope();
        var db = scope.GetService<PostgresDatabase>();
        var connection = await db.ProvideConnection();
        await connection.ExecuteAsync(command);
    }

    private async Task<bool> Ensure_Cleaner_Job_Started(Guid cleanerId, int repeatTries = 10)
    {
        int currentTries = 0;
        while (currentTries < repeatTries)
        {
            var cleaner = await _sp.ScopedExecution(
                scope => scope.GetService<ICleanersStorage>(),
                scope => scope.Get(cleanerId)
            );
            if (cleaner.IsFailure)
            {
                currentTries++;
                await Task.Delay(TimeSpan.FromSeconds(5));
                continue;
            }

            if (cleaner.Value.State == Domain.Cleaners.Aggregate.Cleaner.WorkState)
            {
                return true;
            }

            currentTries++;
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        return false;
    }
}
