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

    private readonly BrowserManagerProvider _browsers;

    public AvitoParserStopper(
        NpgSqlSession session, 
        Serilog.ILogger logger, 
        BrowserManagerProvider browsers)
    {
        _session = session;
        _transactions = new NpgSqlTransactionSource(session);
        _logger = logger.ForContext<AvitoParserStopper>();
        _browsers = browsers;
    }

    public async Task Stop(string Domain, string Type, CancellationToken ct = default)
    {                
        BreakParserProcessThread();
        await ProcessParserFinalizationStageSwitchTransaction(ct);        
        await CleanParserRemainsResources();
        _logger.Information("Finalized");
    }

    private async Task ProcessParserFinalizationStageSwitchTransaction(CancellationToken ct)
    {
        await using(_session)
        {
            await using ITransactionScope transaction = await _transactions.BeginTransaction(ct);
            Maybe<ParserWorkStage> stage = await GetCurrentWorkStageProcess(ct);
            await SwitchToFinalizationStage(stage, ct);
            await transaction.Commit(ct);
        }                                
    }

    private static void BreakParserProcessThread()
    {
        try
        {
            BrowserManager.ForceKillBrowserProcess();
        }
        catch
        {
            
        }
    }

    private async Task CleanParserRemainsResources()
    {
        BrowserManager manager = _browsers.Provide();
        await manager.DisposeAsync();
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
