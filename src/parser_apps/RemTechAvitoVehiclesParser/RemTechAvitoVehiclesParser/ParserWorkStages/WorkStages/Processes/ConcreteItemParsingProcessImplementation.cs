using AvitoFirewallBypass;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
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
            async (deps, ct) =>
            {
                deps.Logger.Information("Concrete items stage");
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope txn = await transactionSource.BeginTransaction(ct);

                WorkStageQuery stageQuery = new(Name: WorkStageConstants.ConcreteItemStageName, WithLock: true);
                Maybe<ParserWorkStage> workStage = await ParserWorkStage.GetSingle(session, stageQuery, ct);
                if (!workStage.HasValue)
                {
                    return;
                }
                
                AvitoVehicle[] items = await FetchVehiclesFromDb(session, ct);
                if (items.Length == 0)
                {
                    await SwitchToTheNextStage(workStage.Value, session, deps.Logger, txn, ct);
                    return;
                }

                await ProcessItemParsingAndPersisting(deps, session, items, ct);
                await CommitTransaction(txn, deps.Logger, ct);
            };
    }

    private static async Task CommitTransaction(ITransactionScope txn, Serilog.ILogger logger, CancellationToken ct)
    {
        Result commit = await txn.Commit(ct);
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Error at committing transaction");
        }
        else
        {
            logger.Information("Transaction committed successfully.");
        }
    }
    
    private static async Task ProcessItemParsingAndPersisting(WorkStageProcessDependencies dependencies, NpgSqlSession session, AvitoVehicle[] items, CancellationToken ct)
    {
        await using BrowserManager manager = dependencies.BrowserManagerProvider.Provide();        
        await foreach (AvitoVehicle[] parsedItems in ProcessItemParsing(dependencies, items, manager).WithCancellation(ct))
        {
            await parsedItems.UpdateFull(session);
        }
    }
    
    private static async Task<AvitoVehicle[]> FetchVehiclesFromDb(NpgSqlSession session, CancellationToken ct)
    {
        AvitoItemQuery itemsQuery = new(UnprocessedOnly: true, Limit: 50, RetryCount: 10, CatalogueOnly: true);
        AvitoVehicle[] items = await AvitoVehicle.GetAsCatalogueRepresentation(session, itemsQuery, ct: ct);
        return items;
    }

    private static async Task SwitchToTheNextStage(ParserWorkStage stage, NpgSqlSession session, Serilog.ILogger logger, ITransactionScope txn, CancellationToken ct)
    {
        stage.ToFinalizationStage();
        await stage.Update(session, ct);
        await txn.Commit(ct);
        logger.Information("Switched to: {Stage}", stage.Name);
    }

    private static async IAsyncEnumerable<AvitoVehicle[]> ProcessItemParsing(WorkStageProcessDependencies dependencies, AvitoVehicle[] items, BrowserManager manager)
    {
        for (int i = 0; i < items.Length; i++)
        {
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
            await using IPage page = await manager.ProvidePage();
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
        }        

        logger.Error("Error at extracting concrete item {Url}", vehicle.CatalogueRepresentation.Url);                        
        return vehicle.IncreaseRetryCount();        
    }
}
