using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.AvitoSpareContext.Extensions;
using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using AvitoSparesParser.Commands.ExtractCataloguePageItems;
using AvitoSparesParser.ParsingStages.Extensions;
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
        public static ParserStageProcess CatalogueItemsExtracting => async (deps, ct) =>
        {
            Serilog.ILogger logger = deps.Logger.ForContext<ParserStageProcess>();
            await using NpgSqlSession session = new(deps.NpgSql);
            NpgSqlTransactionSource source = new(session);
            await using ITransactionScope scope = await source.BeginTransaction(ct);

            Maybe<ParsingStage> stage = await GetCatalogueStage(session, ct);
            if (!stage.HasValue) return;
            
            AvitoCataloguePage[] pages = await GetPaginatedUrls(session, ct);
            if (CanSwitchToNextStage(pages))
            {
                await SwitchToNextStage(logger, stage.Value, session, ct);
                await FinishTransaction(scope, logger, ct);
                return;
            }
            
            await ExtractCataloguePageItems(pages, logger, deps.Browsers, deps.Bypasses, session);
            await FinishTransaction(scope, logger, ct);
        };
    }

    private static async Task FinishTransaction(ITransactionScope scope, Serilog.ILogger logger, CancellationToken ct)
    {
        Result commit = await scope.Commit();
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Failed to commit transaction.");
        }
    }
    
    private static async Task ExtractCataloguePageItems(
        AvitoCataloguePage[] pages, 
        Serilog.ILogger logger, 
        BrowserFactory browsers, 
        AvitoBypassFactory bypass, 
        NpgSqlSession session)
    {
        IBrowser browser = await browsers.ProvideBrowser();
        
        for (int i = 0; i < pages.Length; i++)
        {
            AvitoCataloguePage page = pages[i];
            IExtractCataloguePagesItemCommand command = new ExtractCataloguePagesItemCommand(() => browser.GetPage(), bypass)
                .UseLogging(logger);
            pages[i] = await ProcessExtraction(command, logger, page, session);
        }
        
        await pages.UpdateMany(session);
        await browser.DestroyAsync();
    }
    
    private static async Task<AvitoCataloguePage> ProcessExtraction(
        IExtractCataloguePagesItemCommand command,
        Serilog.ILogger logger, 
        AvitoCataloguePage page,
        NpgSqlSession session)
    {
        try
        {
            AvitoSpare[] items = await command.Extract(page);
            await items.PersistAsCatalogueRepresentationMany(session);
            page.Marker.MarkProcessed();
        }
        catch (EvaluationFailedException ex)
        {
            logger.Fatal(ex, "Evaluation exception in url: {Url}", page.Url);
        }
        catch (Exception)
        {
            page.Counter.Increase();
        }
        
        return page;
    }
    
    private static async Task<Maybe<ParsingStage>> GetCatalogueStage(NpgSqlSession session, CancellationToken ct)
    {
        ParsingStageQuery stageQuery = new(Name: ParsingStageConstants.CATALOGUE, WithLock: true);
        Maybe<ParsingStage> stage = await ParsingStage.GetStage(session, stageQuery, ct);
        return stage;
    }

    private static async Task<AvitoCataloguePage[]> GetPaginatedUrls(NpgSqlSession session, CancellationToken ct)
    {
        AvitoCataloguePageQuery pagesQuery = new(UnprocessedOnly: true, RetryThreshold: 5, WithLock: true);
        AvitoCataloguePage[] pages = await IEnumerable<AvitoCataloguePage>.GetMany(session, pagesQuery, ct);
        return pages;
    }

    private static bool CanSwitchToNextStage(AvitoCataloguePage[] pages)
    {
        return pages.Length == 0;
    }

    private static async Task SwitchToNextStage(Serilog.ILogger logger, ParsingStage stage, NpgSqlSession session, CancellationToken ct)
    {
        ParsingStage concreteItemsStage = stage.ToConcreteItemsStage();
        await concreteItemsStage.Update(session, ct);
        logger.Information("Switched to stage: {Stage}", concreteItemsStage.Name);
    }
}
