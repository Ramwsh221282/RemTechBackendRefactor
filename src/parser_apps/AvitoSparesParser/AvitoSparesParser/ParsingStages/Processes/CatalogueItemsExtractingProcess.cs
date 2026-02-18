using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.AvitoSpareContext.Extensions;
using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using AvitoSparesParser.Commands.ExtractCataloguePageItems;
using AvitoSparesParser.Common;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParsingStages.Processes;

public static class CatalogueItemsExtractingProcess
{
    extension(ParserStageProcess)
    {
        public static ParserStageProcess CatalogueItemsExtracting =>
            async (deps, ct) =>
            {
                Serilog.ILogger logger = deps.Logger.ForContext<ParserStageProcess>();
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource source = new(session);
                await using ITransactionScope scope = await source.BeginTransaction(ct);                

                Maybe<ParsingStage> stage = await GetCatalogueStage(session, ct);
                if (!stage.HasValue)
                {
                    return;
                }

                if (deps.StopState.HasStopBeenRequested())
                {
                    await stage.Value.PermanentFinalize(session, scope, ct);
                    return;
                }

                AvitoCataloguePage[] pages = await GetPaginatedUrls(session, ct);
                if (CanSwitchToNextStage(pages))
                {
                    await SwitchToNextStage(logger, stage.Value, session, ct);
                    await FinishTransaction(scope, logger, ct);
                    return;
                }

                await using (BrowserManager manager = deps.BrowserProvider.Provide())
                {
                    await using (IPage page = await manager.ProvidePage())
                    {
                        await ExtractCataloguePageItems(pages, logger, manager, page, deps.Bypasses, deps.StopState, session);
                    }
                }
                
                
                await FinishTransaction(scope, logger, ct);
            };
    }

    private static async Task FinishTransaction(
        ITransactionScope scope,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        Result commit = await scope.Commit(ct);
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Failed to commit transaction.");
        }
    }

    private static async Task ExtractCataloguePageItems(
        AvitoCataloguePage[] pages,
        Serilog.ILogger logger,
        BrowserManager manager,
        IPage browserPage,
        AvitoBypassFactory bypass,
        ParserStopState stopState,
        NpgSqlSession session
    )
    {
        foreach (AvitoCataloguePage cataloguePage in pages)
        {
            if (stopState.HasStopBeenRequested())
            {                
                break;
            }

            try
            {
                IExtractCataloguePagesItemCommand command = new ExtractCataloguePagesItemCommand(browserPage, bypass)
                    .UseLogging(logger);
                await ProcessExtraction(command, cataloguePage, session);
                cataloguePage.Marker.MarkProcessed();
            }
            catch(Exception)
            {
                cataloguePage.Counter.Increase();
                browserPage = await manager.RecreatePage(browserPage);
                manager.ReleasePageUsedMemoryResources();
            }
        }

        await pages.UpdateMany(session);
    }

    private static async Task ProcessExtraction(
        IExtractCataloguePagesItemCommand command,
        AvitoCataloguePage page,
        NpgSqlSession session
    )
    {
        AvitoSpare[] items = await command.Extract(page);
        await items.PersistAsCatalogueRepresentationMany(session);
    }

    private static async Task<Maybe<ParsingStage>> GetCatalogueStage(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        ParsingStageQuery stageQuery = new(Name: ParsingStageConstants.CATALOGUE, WithLock: true);
        Maybe<ParsingStage> stage = await ParsingStage.GetStage(session, stageQuery, ct);
        return stage;
    }

    private static async Task<AvitoCataloguePage[]> GetPaginatedUrls(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        AvitoCataloguePageQuery pagesQuery = new(
            UnprocessedOnly: true,
            RetryThreshold: 10,
            WithLock: true
        );
        AvitoCataloguePage[] pages = await IEnumerable<AvitoCataloguePage>.GetMany(
            session,
            pagesQuery,
            ct
        );
        return pages;
    }

    private static bool CanSwitchToNextStage(AvitoCataloguePage[] pages)
    {
        return pages.Length == 0;
    }

    private static async Task SwitchToNextStage(
        Serilog.ILogger logger,
        ParsingStage stage,
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        ParsingStage concreteItemsStage = stage.ToConcreteItemsStage();
        await concreteItemsStage.Update(session, ct);
        logger.Information("Switched to stage: {Stage}", concreteItemsStage.Name);
    }
}
