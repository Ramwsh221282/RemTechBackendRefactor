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
        public static ParserStageProcess ConcreteItems =>
            async (deps, ct) =>
            {
                ILogger logger = deps.Logger.ForContext<ParserStageProcess>();
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource source = new(session);
                await using ITransactionScope scope = await source.BeginTransaction(ct);

                Maybe<ParsingStage> stage = await GetConcreteItemsStage(session, ct);
                if (!stage.HasValue)
                    return;

                AvitoSpare[] catalogueSpares = await GetAvitoCatalogueItems(session, ct);
                if (CanSwitchNextStage(catalogueSpares))
                {
                    await SwitchNextStage(stage.Value, session, logger, ct);
                    await FinishTransaction(scope, logger, ct);
                    return;
                }

                await using (BrowserManager manager = deps.BrowserProvider.Provide())
                {
                    await ProcessConcreteItemsExtraction(catalogueSpares, manager, deps, session);
                }

                await FinishTransaction(scope, logger, ct);
            };
    }

    private static async Task<Maybe<ParsingStage>> GetConcreteItemsStage(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        ParsingStageQuery query = new(Name: ParsingStageConstants.CONCRETE_ITEMS, WithLock: true);
        Maybe<ParsingStage> stage = await ParsingStage.GetStage(session, query, ct);
        return stage;
    }

    private static async Task<AvitoSpare[]> GetAvitoCatalogueItems(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        AvitoSpareQuery query = new(
            UnprocessedOnly: true,
            WithLock: true,
            Limit: 50,
            RetryCountThreshold: 10
        );
        return await AvitoSpare.Query(session, query, ct);
    }

    private static bool CanSwitchNextStage(AvitoSpare[] spares)
    {
        return spares.Length == 0;
    }

    private static async Task SwitchNextStage(
        ParsingStage stage,
        NpgSqlSession session,
        ILogger logger,
        CancellationToken ct
    )
    {
        ParsingStage finalization = stage.ToFinalizationStage();
        await finalization.Update(session, ct);
        logger.Information("Switched to {Stage} stage.", finalization.Name);
    }

    private static async Task ProcessConcreteItemsExtraction(AvitoSpare[] spares, BrowserManager manager, ParserStageDependencies deps, NpgSqlSession session)
    {
        for (int i = 0; i < spares.Length; i++)
        {
            spares[i] = await ExtractConcreteSpareFromCatalogueSpare(spares[i], manager, deps.Logger, deps.Bypasses);
        }

        await spares.PersistAsConcreteRepresentationMany(session);
    }

    private static async Task<AvitoSpare> ExtractConcreteSpareFromCatalogueSpare(AvitoSpare spare, BrowserManager manager, ILogger logger, AvitoBypassFactory bypass)
    {                
        int attempts = 0;
        int currentAttempt = 5;
        while (attempts < currentAttempt)
        {
            await using IPage page = await manager.ProvidePage();
            ExtractConcretePageItemCommand command = new(page, bypass);
            try
            {
                AvitoSpare result = await command.UseLogging(logger).Extract(spare);
                return result.MarkProcessed();
            }
            catch (Exception)
            {
                attempts++;
                logger.Warning("Attempt {Attempt} of {TotalAttempts} failed for spare with id: {SpareId}", attempts, currentAttempt, spare.Id);
            }
        }

        return spare.IncreaseRetryAmount();
    }

    private static async Task FinishTransaction(
        ITransactionScope scope,
        ILogger logger,
        CancellationToken ct
    )
    {
        Result commit = await scope.Commit(ct);
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Failed to commit transaction.");
        }
    }
}
