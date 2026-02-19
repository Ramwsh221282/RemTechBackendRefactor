using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.Parsing;
using Quartz;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace AvitoSparesParser.ParsingStages;

[DisallowConcurrentExecution]
[CronSchedule("*/5 * * * * ?")]
public sealed class ParsingStageBackgroundInvoker(ParserStageDependencies dependencies) : ICronScheduleJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await using NpgSqlSession session = new(dependencies.NpgSql);
        NpgSqlTransactionSource transactionSource = new(session);
        await using ITransactionScope transaction = await transactionSource.BeginTransaction(context.CancellationToken);
        Maybe<ParsingStage> stage = await GetStage(session, context.CancellationToken);
        if (stage.HasValue == false)
        {
            dependencies.Logger.Information("No stage to process detected. Ending execution.");
            return;
        }

        if (dependencies.StopState.HasStopBeenRequested())
        {            
            await stage.Value.PermanentFinalize(session, context.CancellationToken);
            return;
        }

        ParserStageProcess process = ParserStageProcessRouter.ResolveStageByName(stage.Value);
        string stageName = stage.Value.Name;        
        try
        {            
            dependencies.Logger.Information("Processing stage {Stage}", stageName);
            await process(dependencies, stage.Value, session, context.CancellationToken);
            Result commit = await transaction.Commit(context.CancellationToken);
            LogTransactionResult(commit, dependencies.Logger);
        }
        catch (Exception e)
        {
            dependencies.Logger.Fatal(e, "Error while processing stage {Stage}", stageName);
        }
    }

    private static void LogTransactionResult(Result result, Serilog.ILogger logger)
    {

        if (result.IsSuccess)
        {
            logger.Information("Transaction commit success");
        }
        else
        {
            logger.Error("Failed to commit transaction. Error: {Error}", result.Error);
        }
    }

    private static async Task<Maybe<ParsingStage>> GetStage(NpgSqlSession session, CancellationToken ct)
    {        
        ParsingStageQuery query = new();
        return await ParsingStage.GetStage(session, query, ct);
    }
}
