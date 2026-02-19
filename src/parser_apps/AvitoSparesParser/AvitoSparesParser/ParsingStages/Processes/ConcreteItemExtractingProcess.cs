using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.AvitoSpareContext.Extensions;
using AvitoSparesParser.Commands.ExtractConcretePageItem;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using ILogger = Serilog.ILogger;

namespace AvitoSparesParser.ParsingStages.Processes;

public static class ConcreteItemExtractingProcess
{
    extension(ParserStageProcess)
    {
        public static ParserStageProcess ConcreteItems =>
            static async (deps, stage, session, ct) =>
            {
                ILogger logger = deps.Logger.ForContext<ParserStageProcess>();                                
                if (deps.StopState.HasStopBeenRequested())
                {
                    await stage.PermanentFinalize(session, ct);
                    return;
                }

                AvitoSpare[] catalogueSpares = await GetAvitoCatalogueItems(session, ct);
                if (CanSwitchNextStage(catalogueSpares))
                {
                    await SwitchNextStage(stage, session, logger, ct);                    
                    return;
                }

                await using BrowserManager manager = deps.BrowserProvider.Provide();
                await ProcessConcreteItemsExtraction(catalogueSpares, manager, deps, session, deps.StopState);
            };
    }    

    private static async Task<AvitoSpare[]> GetAvitoCatalogueItems(NpgSqlSession session, CancellationToken ct)
    {
        AvitoSpareQuery query = new(
            UnprocessedOnly: true,
            WithLock: true,
            Limit: 50,
            RetryCountThreshold: 10);

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
        CancellationToken ct)
    {
        ParsingStage finalization = stage.ToFinalizationStage();
        await finalization.Update(session, ct);
        logger.Information("Switched to {Stage} stage.", finalization.Name);
    }

    private static async Task ProcessConcreteItemsExtraction(
        AvitoSpare[] spares, 
        BrowserManager manager, 
        ParserStageDependencies deps, 
        NpgSqlSession session, 
        ParserStopState state)
    {
        List<AvitoSpare> processedSpares = [];
        List<AvitoSpare> sparesForDelete = [];
        for (int i = 0; i < spares.Length; i++)
        {
            if (state.HasStopBeenRequested())
            {
                break;
            }
            
            (AvitoSpare processed, bool forDelete) = await ExtractConcreteSpareFromCatalogueSpare(spares[i], manager, deps.Logger, deps.Bypasses, state);
            if (forDelete)
            {
                sparesForDelete.Add(processed);
            }
            else
            {
                processedSpares.Add(processed);
            }            
        }

        await processedSpares.PersistAsConcreteRepresentationMany(session);
        await sparesForDelete.DeleteMany(session);        
    }

    private static async Task<(AvitoSpare spare, bool forDelete)> ExtractConcreteSpareFromCatalogueSpare(
        AvitoSpare spare, 
        BrowserManager manager, 
        ILogger logger, 
        AvitoBypassFactory bypass,        
        ParserStopState state)
    {                
        int attempts = 0;
        int currentAttempt = 5;        
        while (attempts < currentAttempt)
        {
            if (state.HasStopBeenRequested())
            {
                break;
            }

            await using IPage page = await manager.ProvidePage();
            ExtractConcretePageItemCommand command = new(page, bypass);
            try
            {
                AvitoSpare result = await command.UseLogging(logger).Extract(spare);
                return (result.MarkProcessed(), false);
            }
            catch (Exception ex)
            {                
                if (ex.Message.Contains("Not all properties set."))
                {                    
                    return (spare, true);
                }

                attempts++;
                logger.Warning("Attempt {Attempt} of {TotalAttempts} failed for spare with id: {SpareId}", attempts, currentAttempt, spare.Id);
            }
        }

        return (spare.IncreaseRetryAmount(), false);
    }    
}
