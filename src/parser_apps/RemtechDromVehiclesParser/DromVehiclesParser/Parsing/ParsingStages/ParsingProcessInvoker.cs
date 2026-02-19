using DromVehiclesParser.Parsers.Database;
using DromVehiclesParser.Parsers.Models;
using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;
using ParsingSDK.Parsing;
using Quartz;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace DromVehiclesParser.Parsing.ParsingStages;

[DisallowConcurrentExecution]
[CronSchedule("*/5 * * * * ?")]
public sealed class ParsingProcessInvoker(ParsingStageDependencies dependencies, NpgSqlConnectionFactory npgsql) : ICronScheduleJob
{
    private ParsingStageDependencies Dependencies { get; } = dependencies;
    
    public async Task Execute(IJobExecutionContext context)
    {
        await using NpgSqlSession session = new(npgsql);
        NpgSqlTransactionSource transactionSource = new(session);        
        CancellationToken ct = context.CancellationToken;
        await using ITransactionScope transaction = await transactionSource.BeginTransaction(ct);        
        Maybe<ParserWorkStage> stage = await GetCurrentParsingStage(session, ct);
        if (!stage.HasValue)        
        {
            Dependencies.Logger.Information("No active parsing stage found. Not invoking parser process.");
            return;
        }

        if (Dependencies.StopState.HasStopBeenRequested())
        {
            await stage.Value.PermanentFinalize(session, Dependencies.StopState, ct);            
            Dependencies.Logger.Information("Stop has been requested. Finishing parser work.");
            return;
        }

        if (!await EnsureHasWorkingParsers(session, ct))
        {
            Dependencies.Logger.Information("No working parsers detected. Not invoking parser process.");
            return;
        }
                
        ParsingStage process = ResolveStage(stage);        
        try
        {
            await process(Dependencies, stage.Value, session, ct);
            Dependencies.Logger.Information("Parsing stage completed.");
            Result commit = await transaction.Commit(ct);
        }
        catch(Exception e)
        {
            Dependencies.Logger.Fatal(e, "Error while invoking parsing stage");
        }
    }

    private static ParsingStage ResolveStage(Maybe<ParserWorkStage> stage)
    {
        if (!stage.HasValue) return ParsingStage.Sleep;
        return ParsingStageRouter.ResolveByStageName(stage.Value);
    }
    
    private static async Task<Maybe<ParserWorkStage>> GetCurrentParsingStage(NpgSqlSession session, CancellationToken ct)
    {
        ParserWorkStageStoringImplementation.ParserWorkStageQuery query = new(WithLock: true);
        return await ParserWorkStage.FromDb(session, query, ct);
    }

    private static async Task<bool> EnsureHasWorkingParsers(NpgSqlSession session, CancellationToken ct)
    {        
        return await WorkingParser.Exists(session, ct);
    }
}