using AvitoFirewallBypass;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Processes;

public static class PaginationExtractionProcessImplementation
{
    extension(WorkStageProcess)
    {
        public static WorkStageProcess PaginationExtraction => async (deps, ct) =>
            {
                Serilog.ILogger logger = deps.Logger.ForContext<WorkStageProcess>();
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope txn = await transactionSource.BeginTransaction(ct);
                
                Maybe<ParserWorkStage> stage = await GetPaginationStage(session, ct);
                if (!stage.HasValue)
                {
                    return;
                }

                if (deps.StopState.HasStopBeenRequested())
                {
                    logger.Information("Stop requested. Finalizing stage.");
                    await stage.Value.PermanentFinalize(session, txn, ct);
                    return;
                }

                ProcessingParserLink[] links = await GetProcessingParserLinksFromDb(session, ct);
                if (links.Length == 0)
                {
                    await SwitchToTheNextStage(stage.Value, session, txn, logger, ct);
                    return;
                }
                
                await using BrowserManager manager = deps.BrowserManagerProvider.Provide();
                await using IPage page = await manager.ProvidePage();
                await foreach (CataloguePageUrl[] results in ProcessPagedUrlsExtraction(deps, links, page, manager).WithCancellation(ct))
                {
                    await results.PersistMany(session);
                }
                
                await links.UpdateMany(session);
                Result commit = await txn.Commit(ct);
                if (commit.IsFailure)
                {
                    logger.Error(commit.Error, "Error at committing transaction");
                }
                else
                {
                    logger.Information("Pagination extracting finished.");
                }
            };
    }

    private static async IAsyncEnumerable<CataloguePageUrl[]> ProcessPagedUrlsExtraction(
        WorkStageProcessDependencies dependencies,
        ProcessingParserLink[] links, 
        IPage page,
        BrowserManager manager)
    {
        for (int i = 0; i < links.Length; i++)
        {
            ProcessingParserLink link = links[i];
            (ProcessingParserLink processingLink, CataloguePageUrl[] pagedUrls) = await ExtractPaginatedCatalogueUrls(dependencies, link, page, manager);
            links[i] = processingLink;
            yield return pagedUrls;
        }
    }
    
    private static async Task<(ProcessingParserLink processingLink, CataloguePageUrl[] pagedUrls)> ExtractPaginatedCatalogueUrls(
        WorkStageProcessDependencies dependencies, 
        ProcessingParserLink link,
        IPage page, 
        BrowserManager manager
        )
    {
        try
        {
            CataloguePageUrl[] results = await ExtractPagination(dependencies, link, page);
            return (link.MarkProcessed(), results);
        }
        catch(Exception)
        {
            page = await manager.RecreatePage(page);
            manager.ReleasePageUsedMemoryResources();
            return (link.IncreaseRetryCount(), Array.Empty<CataloguePageUrl>());
        }
    }

    private static async Task SwitchToTheNextStage(ParserWorkStage stage, NpgSqlSession session, ITransactionScope txn, Serilog.ILogger logger, CancellationToken ct)
    {
        stage.ToCatalogueStage();
        await stage.Update(session, ct);
        await txn.Commit(ct);
        logger.Information("Switched to stage: {Name}", stage.Name);
    }
    
    private static async Task<ProcessingParserLink[]> GetProcessingParserLinksFromDb(NpgSqlSession session, CancellationToken ct)
    {
        ProcessingParserLinkQuery linksQuery = new(UnprocessedOnly: true, RetryLimit: 10, WithLock: true);
        ProcessingParserLink[] links = await ProcessingParserLink.GetMany(session, linksQuery, ct: ct);
        return links;
    }

    private static async Task<Maybe<ParserWorkStage>> GetPaginationStage(NpgSqlSession session, CancellationToken ct)
    {
        string name = WorkStageConstants.EvaluationStageName;
        WorkStageQuery stageQuery = new(Name: name ,WithLock: true);
        Maybe<ParserWorkStage> evalStage = await ParserWorkStage.GetSingle(session, stageQuery, ct);
        return evalStage;
    }
    
    private static async Task<CataloguePageUrl[]> ExtractPagination(WorkStageProcessDependencies dependencies, ProcessingParserLink link, IPage page)
    {
        AvitoPagedUrlsExtractor extractor = new(page, dependencies.Bypasses.Create(page));
        string[] urls = await extractor.ExtractUrls(link.Url);
        CataloguePageUrl[] pagedUrls = [.. urls.Select(CataloguePageUrl.New)];
        dependencies.Logger.Information("Extracted {PageCount} pages for url: {Url}", pagedUrls.Length, link.Url);
        string pagesLogText = string.Join("\n", pagedUrls.Select(p => p.Url));
        dependencies.Logger.Information("Paged Url: {PagedUrl}", pagesLogText);
        return pagedUrls;        
    }

    private sealed class PaginationResult
    {
        public int MaxPage { get; set; }
        public string[] PagedUrls { get; set; } = [];
    }
}
