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
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParsingStages.Processes;

public static class CatalogueItemsExtractingProcess
{
    extension(ParserStageProcess)
    {
        public static ParserStageProcess CatalogueItemsExtracting =>
            async (deps, stage, session, ct) =>
            {
                Serilog.ILogger logger = deps.Logger.ForContext<ParserStageProcess>();                                                                
                if (deps.StopState.HasStopBeenRequested())
                {
                    await stage.PermanentFinalize(session, ct);
                    return;
                }

                AvitoCataloguePage[] pages = await GetPaginatedUrls(session, ct);
                if (CanSwitchToNextStage(pages))
                {
                    await SwitchToNextStage(logger, stage, session, ct);                    
                    return;
                }

                await using BrowserManager manager = deps.BrowserProvider.Provide();                
                await ExtractCataloguePageItems(pages, logger, manager, deps.Bypasses, deps.StopState, session);
                await pages.UpdateMany(session);
            };
    }    

    private static async Task ExtractCataloguePageItems(
        AvitoCataloguePage[] pages,
        Serilog.ILogger logger,
        BrowserManager manager,        
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
                AvitoSpare[] items = await ProcessExtraction(manager, logger, bypass, cataloguePage, stopState);
                await items.PersistAsCatalogueRepresentationMany(session);
                cataloguePage.Marker.MarkProcessed();                
            }
            catch(Exception)
            {
                cataloguePage.Counter.Increase();
                manager.ReleasePageUsedMemoryResources();
            }            
        }        
    }

    private static async Task<AvitoSpare[]> ProcessExtraction(
        BrowserManager manager, 
        Serilog.ILogger logger, 
        AvitoBypassFactory bypass, 
        AvitoCataloguePage page,
        ParserStopState state)
    {
        if (state.HasStopBeenRequested())
        {
            return [];
        }

        IPage browserPage = await manager.ProvidePage();        
        try
        {
            ExtractCataloguePagesItemCommand command = new(browserPage, bypass);                    
            AvitoSpare[] items = await command.UseLogging(logger).Extract(page);
            return items;
        }
        finally
        {
            await browserPage.DisposeAsync();
        }                                
    }    

    private static async Task<AvitoCataloguePage[]> GetPaginatedUrls(NpgSqlSession session, CancellationToken ct)
    {
        AvitoCataloguePageQuery pagesQuery = new(
            UnprocessedOnly: true,
            RetryThreshold: 10,
            WithLock: true);

        AvitoCataloguePage[] pages = await IEnumerable<AvitoCataloguePage>.GetMany(
            session,
            pagesQuery,
            ct);

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
