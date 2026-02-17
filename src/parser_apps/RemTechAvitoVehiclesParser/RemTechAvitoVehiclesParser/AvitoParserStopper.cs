using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser;

public sealed class AvitoParserStopper : IParserStopper
{
    private readonly NpgSqlSession _session;
    private readonly NpgSqlTransactionSource _transactions;
    private readonly Serilog.ILogger _logger;

    public AvitoParserStopper(NpgSqlSession session, Serilog.ILogger logger)
    {
        _session = session;
        _transactions = new NpgSqlTransactionSource(session);
        _logger = logger.ForContext<AvitoParserStopper>();
    }

    public async Task Stop(string Domain, string Type, CancellationToken ct = default)
    {                
        try
        {
            BrowserManager.ForceKillBrowserProcess();
        }
        catch
        {
            
        }

        await using ITransactionScope transaction = await _transactions.BeginTransaction(ct: ct);
        Maybe<ParserWorkStage> stage = await GetCurrentWorkStageProcess(ct);
        await SwitchToFinalizationStage(stage, ct);
        await transaction.Commit(ct);
        _logger.Information("Finalized");
    }

    private async Task<Maybe<ParserWorkStage>> GetCurrentWorkStageProcess(CancellationToken ct)
    {
        WorkStageQuery query = new(WithLock: true);
        Maybe<ParserWorkStage> stage = await ParserWorkStage.GetSingle(_session, query, ct: ct);
        return stage;
    }

    private async Task SwitchToFinalizationStage(Maybe<ParserWorkStage> stage, CancellationToken ct)
    {
        if (!stage.HasValue)
        {
            return;
        }

        ParserWorkStage value = stage.Value;
        value.ToFinalizationStage();
        await value.Update(_session, ct: ct);
    }    
}
