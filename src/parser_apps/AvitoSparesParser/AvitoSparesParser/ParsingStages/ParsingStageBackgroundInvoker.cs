using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.Parsing;
using Quartz;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace AvitoSparesParser.ParsingStages;

[DisallowConcurrentExecution]
[CronSchedule("*/5 * * * * ?")]
public sealed class ParsingStageBackgroundInvoker(ParserStageDependencies dependencies) : ICronScheduleJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Maybe<ParsingStage> stage = await GetStage(context.CancellationToken);
        if (stage.HasValue == false) return;

        ParserStageProcess process = ParserStageProcessRouter.ResolveStageByName(stage.Value);

        try
        {
            dependencies.Logger.Information("Processing stage {Stage}", stage.Value);
            await process(dependencies, context.CancellationToken);
        }
        catch (Exception e)
        {
            dependencies.Logger.Fatal(e, "Error while processing stage {Stage}", stage.Value);
        }
    }

    private async Task<Maybe<ParsingStage>> GetStage(CancellationToken ct)
    {
        await using NpgSqlSession session = new(dependencies.NpgSql);
        ParsingStageQuery query = new();
        return await ParsingStage.GetStage(session, query, ct);
    }
}
