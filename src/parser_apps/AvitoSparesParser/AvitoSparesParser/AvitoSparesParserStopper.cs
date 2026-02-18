using AvitoSparesParser.ParsingStages;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser;

public sealed class AvitoSparesParserStopper : IParserStopper
{
    private readonly NpgSqlSession _session;
    private readonly Serilog.ILogger _logger;
    private readonly BrowserManagerProvider _provider;
    private readonly NpgSqlTransactionSource _transactionSource;

    public AvitoSparesParserStopper(
        NpgSqlSession session,
        Serilog.ILogger logger,
        BrowserManagerProvider provider)
    {
        _session = session;
        _logger = logger;
        _provider = provider;
        _transactionSource = new(session);
    }

    public async Task Stop(string Domain, string Type, CancellationToken ct = default)
    {
        BreakParserThread();
        await PerformParserFinalizationSwitchTransaction(ct);
        await CleanParserRemains();
        _logger.Information("Finalized");
    }

    private async Task PerformParserFinalizationSwitchTransaction(CancellationToken ct)
    {
        await using (_session)
        {
            await using ITransactionScope txn = await _transactionSource.BeginTransaction(ct);
            Maybe<ParsingStage> stage = await GetCurrentParsingStage(_session, ct);                                    
            await SwitchToFinalizationStage(_session, stage, ct);            
        }        
    }

    private static async Task SwitchToFinalizationStage(
        NpgSqlSession session, 
        Maybe<ParsingStage> stage,
        CancellationToken ct)
    {
        if (!stage.HasValue)
        {
            return;
        }

        ParsingStage value = stage.Value;
        ParsingStage finalized = value.ToFinalizationStage();
        await finalized.Update(session, ct);        
    }

    private static async Task<Maybe<ParsingStage>> GetCurrentParsingStage(
        NpgSqlSession session, 
        CancellationToken ct)
    {
        ParsingStageQuery query = new(WithLock: true);
        return await ParsingStage.GetStage(session, query, ct);
    }

    private static void BreakParserThread()
    {
        try
        {
            BrowserManager.ForceKillBrowserProcess();
        }
        catch
        {
            
        }
    }

    private async Task CleanParserRemains()
    {
        BrowserManager manager = _provider.Provide();
        await manager.DisposeAsync();
    }
}
