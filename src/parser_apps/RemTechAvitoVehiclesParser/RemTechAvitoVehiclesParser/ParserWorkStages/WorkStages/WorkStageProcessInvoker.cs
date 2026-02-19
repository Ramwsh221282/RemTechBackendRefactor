using ParsingSDK.Parsing;
using Quartz;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages;

[DisallowConcurrentExecution]
[CronSchedule("*/5 * * * * ?")]
public sealed class WorkStageProcessInvoker(WorkStageProcessDependencies deps, NpgSqlConnectionFactory npgSql) : ICronScheduleJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await using NpgSqlSession session = new(npgSql);
        NpgSqlTransactionSource transactionSource = new(session);
        ITransactionScope transaction = await transactionSource.BeginTransaction(context.CancellationToken);
        Maybe<ParserWorkStage> workStage = await GetWorkStage(session, context.CancellationToken);
        if (!workStage.HasValue)
        {
            return;            
        }        

        if (deps.StopState.HasStopBeenRequested())
        {
            await workStage.Value.PermanentFinalize(session, deps.StopState, context.CancellationToken);
            return;
        }

        try
        {
            WorkStageProcess process = WorkStageProcessRouter.Route(workStage.Value);
            await process(deps, workStage.Value, session, context.CancellationToken);
            Result commit = await transaction.Commit(context.CancellationToken);
            LogTransactionResult(commit, deps.Logger);
        }
        catch(Exception ex)
        {
            deps.Logger.Error(ex, "Error while processing work stage with id {WorkStageId}", workStage.Value.Id);
        }        
    }

    private static async Task<Maybe<ParserWorkStage>> GetWorkStage(NpgSqlSession session, CancellationToken ct)
    {
        WorkStageQuery query = new();        
        Maybe<ParserWorkStage> workStage = await ParserWorkStage.GetSingle(session, query, ct);
        return workStage;
    }

    private static void LogTransactionResult(Result result, Serilog.ILogger logger)
    {
        if (result.IsSuccess)
        {
            logger.Information("Transaction completed successfully");
        }
        else
        {
            logger.Error("Transaction failed with error: {ErrorMessage}", result.Error.Message);
        }
    }
}
