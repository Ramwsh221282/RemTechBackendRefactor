using DromVehiclesParser.Commands.ExtractAdvertisementsFromCatalogue;
using DromVehiclesParser.Commands.HoverCatalogueImages;
using DromVehiclesParser.Parsing.CatalogueParsing.Extensions;
using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public static class CatalogueAdvertisementsExtractingStage
{
    extension(ParsingStage)
    {
        public static ParsingStage CatalogueAdvertisementsExtraction =>
            async (deps, ct) =>
            {
                NpgSqlConnectionFactory npgSql = deps.NpgSql;
                Serilog.ILogger logger = deps.Logger;

                await using NpgSqlSession session = new(npgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope transaction = await transactionSource.BeginTransaction(ct);

                Maybe<ParserWorkStage> stage = await GetCatalogueStage(session, ct);
                if (!stage.HasValue)
                {
                    return;
                }

                if (deps.StopState.HasStopBeenRequested())
                {
                    await stage.Value.PermanentFinalize(session, transaction, ct);
                    logger.Information("Stop has been requested. Finishing parser work.");
                    return;
                }

                DromCataloguePage[] pages = await GetCataloguePages(session, ct);
                if (CanSwitchNextStage(pages))
                {
                    await SwitchNextStage(stage, session, logger, ct);
                    await FinishTransaction(transaction, logger, ct);
                    return;
                }

                await using BrowserManager manager = deps.Browsers.Provide();                                
                await ExtractCatalogueAdvertisements(pages, manager, session, logger, deps.StopState);
                await FinishTransaction(transaction, logger, ct);
            };
    }

    private static async Task ExtractCatalogueAdvertisements(
        DromCataloguePage[] pages,
        BrowserManager manager,        
        NpgSqlSession session,
        Serilog.ILogger logger,
        ParserStopState state
    )
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (state.HasStopBeenRequested())
            {
                logger.Information("Stop has been requested. Finishing parser work.");
                break;
            }

            DromCataloguePage dromCataloguePage = pages[i];
            pages[i] = await ProcessExtraction(manager, logger, dromCataloguePage, session);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        
        await pages.UpdateMany(session);
    }

    private static async Task<DromCataloguePage> ProcessExtraction(
        BrowserManager manager,        
        Serilog.ILogger logger,
        DromCataloguePage page,
        NpgSqlSession session
    )
    {
        int attempts = 5;
        int currentAttempts = 0;
        while(attempts < currentAttempts)
        {
            await using IPage webPage = await manager.ProvidePage();
            ExtractAdvertisementsFromCatalogueCommand extract = new(webPage);
            HoverAdvertisementsCatalogueImagesCommand hover = new(webPage);

            try
            {
                DromCatalogueAdvertisement[] advertisements = await extract.UseLogging(logger).Extract(page, hover.UseLogging(logger));
                await advertisements.PersistMany(session);
                return page.MarkProcessed();
            }
            catch(Exception)
            {
                logger.Warning("Ошибка при извлечении объявлений с страницы {Url}. Попытка номер {Attempt}", page.Url, currentAttempts + 1);
                currentAttempts++;
            }
        }

        return page.IncreaseRetryCount();
    }

    private static async Task FinishTransaction(
        ITransactionScope transaction,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        Result result = await transaction.Commit(ct);
        if (result.IsFailure)
        {
            logger.Fatal(result.Error, "Failed to commit transaction");
        }
    }

    private static async Task SwitchNextStage(
        Maybe<ParserWorkStage> stage,
        NpgSqlSession session,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        ParserWorkStage concreteAdvertisementsStage = stage.Value.ConcreteStage();
        await concreteAdvertisementsStage.Update(session, ct);
        logger.Information("Switched to stage: {Name}", concreteAdvertisementsStage.StageName);
    }

    private static async Task<Maybe<ParserWorkStage>> GetCatalogueStage(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        ParserWorkStageStoringImplementation.ParserWorkStageQuery query = new(
            Name: ParserWorkStageConstants.CATALOGUE,
            WithLock: true
        );
        Maybe<ParserWorkStage> stage = await ParserWorkStage.FromDb(session, query, ct);
        return stage;
    }

    private static async Task<DromCataloguePage[]> GetCataloguePages(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        DromCataloguePageQuery query = new(UnprocessedOnly: true, RetryLimit: 10, WithLock: true);
        return await DromCataloguePage.GetMany(session, query, ct);
    }

    private static bool CanSwitchNextStage(DromCataloguePage[] pages)
    {
        return pages.Length == 0;
    }
}
