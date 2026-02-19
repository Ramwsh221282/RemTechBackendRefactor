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
            async (WorkStageProcessDependencies deps, 
                  ParserWorkStage stage, 
                  NpgSqlSession session, 
                  CancellationToken ct) =>
            {                                                             
                if (deps.StopState.HasStopBeenRequested())
                {
                    await stage.PermanentFinalize(session, deps.StopState, ct);
                    return;
                }
                
                CataloguePageUrl[] urls = await RetrieveCataloguePagedUrlsFromDb(session);
                if (urls.Length == 0)
                {
                    await SwitchToTheNextStage(stage, session, deps.Logger, ct);
                    return;
                }
                
                await ProcessExtractionAndPersistance(deps, session, urls, deps.StopState);                
            };
    }

    private static async Task SwitchToTheNextStage(
        ParserWorkStage workStage, 
        NpgSqlSession session,
        Serilog.ILogger logger,        
        CancellationToken ct)
    {
        workStage.ToConcreteStage();
        await workStage.Update(session, ct);        
        logger.Information("Switched to stage: {Stage}", workStage.Name);
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
            (AvitoVehicle[] items, CataloguePageUrl processedUrl) = await ProcessPage(manager, page, dependencies, url);
            urls[i] = processedUrl;
            yield return items;
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
        AvitoVehicle[] items = [];
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
