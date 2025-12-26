using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Dapper;
using Quartz;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaner.Adapter.JobScheduler;

public sealed class CleanerInvokeJob(
    PostgresDatabase database,
    ICommandHandler<StartWorkCommand, Status<Cleaners.Domain.Cleaners.Aggregate.Cleaner>> handler,
    Serilog.ILogger logger
) : IJob
{
    private const string Context = nameof(CleanerInvokeJob);

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await Handle();
        }
        catch (Exception ex)
        {
            logger.Fatal("{Context}. Fatal at executing job. Error: {Exception}.", Context, ex);
        }
    }

    private async Task Handle()
    {
        logger.Information("{Context}. Attempt to invoke cleaner job.", Context);
        Guid? requiredCleanerId = await GetRequiredCleaner();
        if (requiredCleanerId == null)
        {
            logger.Warning("{Context}. No cleaners for work found.", Context);
            return;
        }

        logger.Information(
            "{Context}. Found cleaner with ID: {Id} ready for job.",
            Context,
            requiredCleanerId
        );
        var command = new StartWorkCommand(requiredCleanerId.Value);
        var result = await handler.Handle(command);

        if (result.IsFailure)
        {
            logger.Error("{Context}. Unable to start cleaner job. {Error}.", Context, result.Error);
        }
        else
        {
            logger.Information("{Context}. Successfully started cleaner job.", Context);
        }
    }

    private async Task<Guid?> GetRequiredCleaner()
    {
        string state = Cleaners.Domain.Cleaners.Aggregate.Cleaner.WorkState;
        const string sql = """
            SELECT 
            id
            FROM cleaners_module.cleaners
            WHERE 
                next_run IS NOT NULL AND 
                next_run <= NOW() AT TIME ZONE 'UTC' AND
                state != @state
            LIMIT 1
            """;
        CommandDefinition command = new(sql, new { state });
        using var connection = await database.ProvideConnection();
        return await connection.QueryFirstOrDefaultAsync<Guid?>(command);
    }
}
