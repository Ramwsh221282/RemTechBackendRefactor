using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.AvitoSpareContext.Extensions;
using AvitoSparesParser.Commands.ExtractConcretePageItem;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using ILogger = Serilog.ILogger;

namespace AvitoSparesParser.ParsingStages.Processes;

public static class ConcreteItemExtractingProcess
{
    extension(ParserStageProcess)
    {
        public static ParserStageProcess ConcreteItems => async (deps, ct) =>
        {
            ILogger logger = deps.Logger.ForContext<ParserStageProcess>();
            await using NpgSqlSession session = new(deps.NpgSql);
            NpgSqlTransactionSource source = new(session, logger);
            await using ITransactionScope scope = await source.BeginTransaction(ct);
            
            Maybe<ParsingStage> stage = await GetConcreteItemsStage(session, ct);
            if (!stage.HasValue) return;
            
            AvitoSpare[] catalogueSpares = await GetAvitoCatalogueItems(session, ct);
            if (CanSwitchNextStage(catalogueSpares))
            {
                await SwitchNextStage(stage.Value, session, logger, ct);
                await FinishTransaction(scope, logger, ct);
                return;
            }
            
            await ProcessConcreteItemsExtraction(catalogueSpares, deps, session);
            await FinishTransaction(scope, logger, ct);
        };
    }
    
    private static async Task<Maybe<ParsingStage>> GetConcreteItemsStage(NpgSqlSession session, CancellationToken ct)
    {
        ParsingStageQuery query = new(Name: ParsingStageConstants.CONCRETE_ITEMS, WithLock: true);
        Maybe<ParsingStage> stage = await ParsingStage.GetStage(session, query, ct);
        return stage;
    }

    private static async Task<AvitoSpare[]> GetAvitoCatalogueItems(NpgSqlSession session, CancellationToken ct)
    {
        AvitoSpareQuery query = new(UnprocessedOnly: true, WithLock: true, Limit: 50, RetryCountThreshold: 10);
        return await AvitoSpare.Query(session, query, ct);
    }

    private static bool CanSwitchNextStage(AvitoSpare[] spares)
    {
        return spares.Length == 0;
    }
    
    private static async Task SwitchNextStage(ParsingStage stage, NpgSqlSession session, ILogger logger, CancellationToken ct)
    {
        ParsingStage finalization = stage.ToFinalizationStage();
        await finalization.Update(session, ct);
        logger.Information("Switched to {Stage} stage.", finalization.Name);
    }

    private static async Task ProcessConcreteItemsExtraction(AvitoSpare[] spares, ParserStageDependencies deps, NpgSqlSession session)
    {
        IBrowser browser = await deps.Browsers.ProvideBrowser();
        for (int i = 0; i < spares.Length; i++)
        {
            AvitoSpare spare = spares[i];
            
            IExtractConcretePageItemCommand command = new ExtractConcretePageItemCommand(() => browser.GetPage(), deps.Bypasses)
                .UseLogging(deps.Logger);
            
            spares[i] = await ExtractConcreteSpareFromCatalogueSpare(command, spare);
        }
        await spares.PersistAsConcreteRepresentationMany(session);
        await browser.DestroyAsync();
    }

    private static async Task<AvitoSpare> ExtractConcreteSpareFromCatalogueSpare(
        IExtractConcretePageItemCommand command,
        AvitoSpare spare
        )
    {
        try
        {
            spare = await command.Extract(spare);
            return spare.MarkProcessed();
        }
        catch(EvaluationFailedException)
        {
            return spare;
        }
        catch (Exception)
        {
            return spare.IncreaseRetryAmount();
        }
    }

    private static async Task FinishTransaction(ITransactionScope scope, ILogger logger, CancellationToken ct)
    {
        Result commit = await scope.Commit();
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Failed to commit transaction.");
        }
    }
}