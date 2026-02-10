using DromVehiclesParser.Commands.ExtractAdvertisementFromItsPage;
using DromVehiclesParser.Parsing.CatalogueParsing.Extensions;
using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using DromVehiclesParser.Parsing.ConcreteItemParsing.Extensions;
using DromVehiclesParser.Parsing.ConcreteItemParsing.Models;
using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public static class AdvertisementsExtactingFromitsPageStage
{
    extension(ParsingStage)
    {
        public static ParsingStage ExtractAdvertisementsFromItsPage =>
            async (ParsingStageDependencies deps, CancellationToken ct) =>
            {
                Serilog.ILogger logger = deps.Logger.ForContext<ParsingStage>();
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope transaction = await transactionSource.BeginTransaction(ct);

                Maybe<ParserWorkStage> stage = await GetConcreteStage(session);
                if (!stage.HasValue)
                {
                    return;
                }

                DromCatalogueAdvertisement[] advertisements = await GetCatalogueAdvertisementsForProcessing(session);
                if (CanSwitchNextStage(advertisements))
                {
                    await SwitchNextStage(stage, session, logger, ct);
                    await FinishTransaction(transaction, logger, ct);
                    return;
                }

                await using BrowserManager manager = deps.Browsers.Provide();
                await using IPage page = await manager.ProvidePage();
                
                await ExtractAdvertisementsFromItsPage(advertisements, manager, page, session, logger);
                await FinishTransaction(transaction, logger, ct);
            };
    }

    private static async Task ExtractAdvertisementsFromItsPage(
        DromCatalogueAdvertisement[] advertisements,
        BrowserManager manager,
        IPage page,
        NpgSqlSession session,
        Serilog.ILogger logger
    )
    {
        
        List<DromAdvertisementFromPage> results = [];
        for (int i = 0; i < advertisements.Length; i++)
        {
            DromCatalogueAdvertisement advertisement = advertisements[i];
            
            try
            {
                await ExtractAdvertisementFromItsPage(manager, page, logger, advertisement, results);
                advertisement = advertisement.MarkProcessed();
                await advertisement.Update(session);
                logger.Information("Extracted advertisement from page {Url}", advertisement.Url);
            }
            catch (WithdrawException)
            {
                logger.Information("Advertisement {Url} withdrawn from sale", advertisement.Url);
                await advertisement.Remove(session);
            }
            catch (Exception)
            {
                logger.Fatal("Failed to extract advertisement from page {Url}", advertisement.Url);
                advertisement = advertisement.IncrementRetryCount();
            }
            finally
            {
                advertisements[i] = advertisement;
                await advertisement.Update(session);
                await Task.Delay(TimeSpan.FromSeconds(5)); // forced delay to avoid get blocked.
            }
        }
        
        await results.PersistMany(session);
    }

    private static async Task ExtractAdvertisementFromItsPage(
        BrowserManager manager, 
        IPage page, 
        Serilog.ILogger logger,
        DromCatalogueAdvertisement advertisement,
        List<DromAdvertisementFromPage> source)
    {
        ExtractAdvertisementFromItsPageCommand command = new(page);
        DromAdvertisementFromPage result = await command.UseLogging(logger).UseResilience(page, manager).Extract(advertisement);
        source.Add(result);
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
        ParserWorkStage concreteAdvertisementsStage = stage.Value.FinalizationStage();
        await concreteAdvertisementsStage.Update(session, ct);
        logger.Information("Switched to stage: {Name}", concreteAdvertisementsStage.StageName);
    }

    private static bool CanSwitchNextStage(DromCatalogueAdvertisement[] advertisements)
    {
        return advertisements.Length == 0;
    }

    private static async Task<DromCatalogueAdvertisement[]> GetCatalogueAdvertisementsForProcessing(
        NpgSqlSession session
    )
    {
        DromCatalogueAdvertisementQuery query = new(
            UnprocessedOnly: true,
            WithLock: true,
            RetryLimit: 10,
            Limit: 20
        );
        return await DromCatalogueAdvertisement.GetMany(session, query, CancellationToken.None);
    }

    private static async Task<Maybe<ParserWorkStage>> GetConcreteStage(NpgSqlSession session)
    {
        ParserWorkStageStoringImplementation.ParserWorkStageQuery query = new(
            Name: ParserWorkStageConstants.CONCRETE,
            WithLock: true
        );
        Maybe<ParserWorkStage> stage = await ParserWorkStage.FromDb(session, query);
        return stage;
    }
}
