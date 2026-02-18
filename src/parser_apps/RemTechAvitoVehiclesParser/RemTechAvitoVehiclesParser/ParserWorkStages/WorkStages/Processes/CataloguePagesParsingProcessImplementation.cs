using AvitoFirewallBypass;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractCatalogueItemData;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Processes;

public static class CataloguePagesParsingProcessImplementation
{
    extension(WorkStageProcess)
    {
        public static WorkStageProcess CatalogueProcess =>
            async (WorkStageProcessDependencies deps, CancellationToken ct) =>
            {
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope txn = await transactionSource.BeginTransaction(ct);
                
                Maybe<ParserWorkStage> stage = await GetCatalogueStage(session, ct);
                if (!stage.HasValue)
                {
                    return;
                }

                if (deps.StopState.HasStopBeenRequested())
                {
                    await stage.Value.PermanentFinalize(session, txn, ct);
                    return;
                }
                
                CataloguePageUrl[] urls = await RetrieveCataloguePagedUrlsFromDb(session);
                if (urls.Length == 0)
                {
                    await SwitchToTheNextStage(stage.Value, session, deps.Logger, txn, ct);
                    return;
                }
                
                await ProcessExtractionAndPersistance(deps, session, urls, deps.StopState);
                Result commit = await txn.Commit(ct);
                if (commit.IsFailure)
                {
                    deps.Logger.Error(commit.Error, "Error at committing transaction");
                }
                else
                {
                    deps.Logger.Information("Transaction committed successfully.");
                }
            };
    }

    private static async Task SwitchToTheNextStage(
        ParserWorkStage workStage, 
        NpgSqlSession session,
        Serilog.ILogger logger,
        ITransactionScope transaction, 
        CancellationToken ct)
    {
        workStage.ToConcreteStage();
        await workStage.Update(session, ct);
        await transaction.Commit(ct);
        logger.Information("Switched to stage: {Stage}", workStage.Name);
    }
    
    private static async Task<Maybe<ParserWorkStage>> GetCatalogueStage(NpgSqlSession session, CancellationToken ct)
    {
        WorkStageQuery stageQuery = new(Name: WorkStageConstants.CatalogueStageName, WithLock: true);
        Maybe<ParserWorkStage> stage = await ParserWorkStage.GetSingle(session, stageQuery, ct);
        return stage;
    }
    
    private static async Task<CataloguePageUrl[]> RetrieveCataloguePagedUrlsFromDb(NpgSqlSession session)
    {
        CataloguePageUrlQuery pageUrlQuery = new(UnprocessedOnly: true, RetryLimit: 10, WithLock: true, Limit: 20);
        return await CataloguePageUrl.GetMany(session, pageUrlQuery);
    }
    
    private static async Task ProcessExtractionAndPersistance(
        WorkStageProcessDependencies dependencies, 
        NpgSqlSession session, 
        CataloguePageUrl[] urls,
        ParserStopState stopState)
    {
        await using BrowserManager manager = dependencies.BrowserManagerProvider.Provide();
        await using IPage page = await manager.ProvidePage();
        await foreach (AvitoVehicle[] items in ExtractCatalogueItems(urls, manager, page, dependencies))
        {
            if (stopState.HasStopBeenRequested())
            {
                dependencies.Logger.Information("Stop requested. Ending processing catalogue pages.");
                break;
            }

            if (items.Length > 0)
            {
                await items.PersistAsCatalogueRepresentation(session);
            }
        }

        await urls.UpdateMany(session);
    }
    
    private static async IAsyncEnumerable<AvitoVehicle[]> ExtractCatalogueItems(
        CataloguePageUrl[] urls,
        BrowserManager manager,
        IPage page,
        WorkStageProcessDependencies dependencies
        )
    {
        for (int i = 0; i < urls.Length; i++)
        {
            CataloguePageUrl url = urls[i];
            (AvitoVehicle[] items, CataloguePageUrl processedUrl) result = await ProcessPage(manager, page, dependencies, url);
            urls[i] = result.processedUrl;
            yield return result.items;
        }
    }

    private static async Task<(AvitoVehicle[] ExtractedItems, CataloguePageUrl ProcessedUrl)> ProcessPage(
        BrowserManager manager,
        IPage page,
        WorkStageProcessDependencies dependencies,
        CataloguePageUrl url)
    {
        AvitoBypassFactory bypass = dependencies.Bypasses;
        Serilog.ILogger logger = dependencies.Logger;
        ExtractCatalogueItemDataCommand command = new(page, url, bypass);
        AvitoVehicle[] items = Array.Empty<AvitoVehicle>();
        try
        {
            items = await command.UseLogging(logger).UseResilience(manager, page, logger).Handle();
            return (items, url.MarkProcessed());
        }
        catch(Exception)
        {
            return (items, url.IncrementRetryCount());
        }
    }
}
