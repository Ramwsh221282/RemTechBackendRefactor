using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser;


public sealed class DromParserStopper : IParserStopper
{
    private readonly NpgSqlSession _session;    
    private readonly Serilog.ILogger _logger;
    private readonly BrowserManagerProvider _browsers;

    public DromParserStopper(
        NpgSqlSession session, 
        Serilog.ILogger logger,
        BrowserManagerProvider browsers)
    {
        _session = session;
        _logger = logger.ForContext<DromParserStopper>();
        _browsers = browsers;
    }

    public async Task Stop(string Domain, string Type, CancellationToken ct = default)
    {
        BreakParsingProcessThread();
        await ProcessParserWorkStageSwitchTransaction(_session, ct);                
        await CleanParserRemains(_browsers);
        _logger.Information("Парсер успешно остановлен. Домен: {Domain}, Тип: {Type}", Domain, Type);
    }

    private static async Task CleanParserRemains(BrowserManagerProvider provider)
    {
        BrowserManager manager = provider.Provide();
        await manager.DisposeAsync();
    }

    private static void BreakParsingProcessThread()
    {
        try
        {
            BrowserManager.ForceKillBrowserProcess();
        }
        catch
        {
            
        }
    }

    private static async Task ProcessParserWorkStageSwitchTransaction(NpgSqlSession session, CancellationToken ct)
    {
        await using (session)
        {
            NpgSqlTransactionSource transactionSource = new(session);
            ITransactionScope transaction = await transactionSource.BeginTransaction(ct);
            Maybe<ParserWorkStage> stage = await GetCurrentWorkStage(session, ct);
            await ProcessParserWorkStageSwitch(session, stage);
            await transaction.Commit(ct);
        }
    }

    private static async Task<Maybe<ParserWorkStage>> GetCurrentWorkStage(NpgSqlSession session, CancellationToken ct)
    {
        ParserWorkStageStoringImplementation.ParserWorkStageQuery query = new(WithLock: true);
        return await ParserWorkStage.FromDb(session, query, ct);
    }

    private static async Task ProcessParserWorkStageSwitch(NpgSqlSession session, Maybe<ParserWorkStage> workStage)
    {
        if (!workStage.HasValue)
        {
            return;
        }

        ParserWorkStage value = workStage.Value;
        ParserWorkStage finalized = value.FinalizationStage();
        await finalized.Update(session);
    }    
}
