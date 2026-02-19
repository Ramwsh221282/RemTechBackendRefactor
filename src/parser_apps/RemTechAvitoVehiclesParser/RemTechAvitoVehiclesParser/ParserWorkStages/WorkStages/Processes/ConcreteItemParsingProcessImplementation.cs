using AvitoFirewallBypass;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractConcreteItem;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Processes;

public static class ConcreteItemParsingProcessImplementation
{
    extension(WorkStageProcess)
    {
        public static WorkStageProcess ConcreteItems =>
            async (deps, stage, session, ct) =>
            {
                deps.Logger.Information("Concrete items stage");                                
                if (deps.StopState.HasStopBeenRequested())
                {
                    await stage.PermanentFinalize(session, deps.StopState, ct);
                    return;
                }
                
                AvitoVehicle[] items = await FetchVehiclesFromDb(session, ct);
                if (items.Length == 0)
                {
                    await SwitchToTheNextStage(stage, session, deps.Logger, ct);
                    return;
                }

                await ProcessItemParsingAndPersisting(deps, session, items, deps.StopState, ct);                
            };
    }    
    
    private static async Task ProcessItemParsingAndPersisting(
        WorkStageProcessDependencies dependencies, 
        NpgSqlSession session, 
        AvitoVehicle[] items, 
        ParserStopState stopState, 
        CancellationToken ct)
    {
        await using BrowserManager manager = dependencies.BrowserManagerProvider.Provide();        
        await foreach (AvitoVehicle[] parsedItems in ProcessItemParsing(dependencies, items, manager, stopState).WithCancellation(ct))
        {
            if (stopState.HasStopBeenRequested())
            {
                dependencies.Logger.Information("Stop requested. Ending processing concrete items.");
                break;
            }

            await parsedItems.UpdateFull(session);
        }
    }
    
    private static async Task<AvitoVehicle[]> FetchVehiclesFromDb(NpgSqlSession session, CancellationToken ct)
    {
        AvitoItemQuery itemsQuery = new(UnprocessedOnly: true, Limit: 50, RetryCount: 9, CatalogueOnly: true);
        AvitoVehicle[] items = await AvitoVehicle.GetAsCatalogueRepresentation(session, itemsQuery, ct: ct);
        return items;
    }

    private static async Task SwitchToTheNextStage(
        ParserWorkStage stage, 
        NpgSqlSession session, 
        Serilog.ILogger logger, 
        CancellationToken ct)
    {
        stage.ToFinalizationStage();
        await stage.Update(session, ct);        
        logger.Information("Switched to: {Stage}", stage.Name);
    }

    private static async IAsyncEnumerable<AvitoVehicle[]> ProcessItemParsing(
        WorkStageProcessDependencies dependencies, 
        AvitoVehicle[] items, 
        BrowserManager manager, 
        ParserStopState stopState)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (stopState.HasStopBeenRequested())
            {
                dependencies.Logger.Information("Stop requested. Ending processing concrete items.");
                break;
            }

            AvitoVehicle item = items[i];
            item = await Scraped(manager, dependencies, item);
            items[i] = item;
        }
        
        yield return items;
    }
    
    private static async Task<AvitoVehicle> Scraped(BrowserManager manager, WorkStageProcessDependencies dependencies, AvitoVehicle vehicle)
    {
        Serilog.ILogger logger = dependencies.Logger;
        AvitoBypassFactory bypasses = dependencies.Bypasses;
        int attempts = 5;
        int currentAttempt = 0;
        while(currentAttempt < attempts)
        {
            IPage page = await manager.ProvidePage();
            ExtractConcreteItemCommand command = new(page, vehicle, bypasses);        
            try
            {
                AvitoVehicle result = await command.UseLogging(logger).Handle();    
                return result.MarkProcessed();
            }            
            catch(Exception)
            {
                currentAttempt++;
            }                        
            finally
            {
                await page.DisposeAsync();                
            }            
        }        

        logger.Error("Error at extracting concrete item {Url}", vehicle.CatalogueRepresentation.Url);                        
        return vehicle.IncreaseRetryCount();        
    }
}
