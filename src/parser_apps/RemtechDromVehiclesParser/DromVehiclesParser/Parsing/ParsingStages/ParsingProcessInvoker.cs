using DromVehiclesParser.Parsers.Database;
using DromVehiclesParser.Parsers.Models;
using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;
using ParsingSDK.Parsing;
using Quartz;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace DromVehiclesParser.Parsing.ParsingStages;

[DisallowConcurrentExecution]
[CronSchedule("*/5 * * * * ?")]
public sealed class ParsingProcessInvoker(ParsingStageDependencies dependencies) : ICronScheduleJob
{
    private ParsingStageDependencies Dependencies { get; } = dependencies;
    
    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken ct = context.CancellationToken;
        if (!await EnsureHasWorkingParsers(ct))
        {
            Dependencies.Logger.Information("No working parsers detected. Not invoking parser process.");
            return;
        }
        
        Maybe<ParserWorkStage> stage = await GetCurrentParsingStage(ct);
        ParsingStage process = ResolveStage(stage);
        
        try
        {
            await process(Dependencies, ct);
            Dependencies.Logger.Information("Parsing stage completed.");
        }
        catch(Exception e)
        {
            Dependencies.Logger.Fatal(e, "Error while invoking parsing stage");
        }
    }

    private ParsingStage ResolveStage(Maybe<ParserWorkStage> stage)
    {
        if (!stage.HasValue) return ParsingStage.Sleep;
        return ParsingStageRouter.ResolveByStageName(stage.Value);
    }
    
    private async Task<Maybe<ParserWorkStage>> GetCurrentParsingStage(CancellationToken ct)
    {
        await using NpgSqlSession session = new(Dependencies.NpgSql);
        var query = new ParserWorkStageStoringImplementation.ParserWorkStageQuery();
        return await ParserWorkStage.FromDb(session, query, ct);
    }

    private async Task<bool> EnsureHasWorkingParsers(CancellationToken ct)
    {
        await using NpgSqlSession session = new(Dependencies.NpgSql);
        return await WorkingParser.Exists(session, ct);
    }
}