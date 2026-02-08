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
        public static WorkStageProcess PaginationExtraction =>
            async (deps, ct) =>
            {
                Serilog.ILogger logger = deps.Logger.ForContext<WorkStageProcess>();
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope txn = await transactionSource.BeginTransaction(ct);
                WorkStageQuery stageQuery = new(
                    Name: WorkStageConstants.EvaluationStageName,
                    WithLock: true
                );
                Maybe<ParserWorkStage> evalStage = await ParserWorkStage.GetSingle(
                    session,
                    stageQuery,
                    ct
                );
                if (!evalStage.HasValue)
                    return;

                ProcessingParserLinkQuery linksQuery = new(
                    UnprocessedOnly: true,
                    RetryLimit: 10,
                    WithLock: true
                );
                ProcessingParserLink[] links = await ProcessingParserLink.GetMany(
                    session,
                    linksQuery,
                    ct: ct
                );
                if (links.Length == 0)
                {
                    evalStage.Value.ToCatalogueStage();
                    await evalStage.Value.Update(session, ct);
                    await txn.Commit(ct);
                    logger.Information("Switched to stage: {Name}", evalStage.Value.Name);
                    return;
                }

                logger.Information("Starting extracting pagination for links.");

                try
                {
                    IBrowser browser = await deps.Browsers.ProvideBrowser();
                    for (int i = 0; i < links.Length; i++)
                    {
                        ProcessingParserLink link = links[i];
                        try
                        {
                            IPage page = await browser.GetPage();

                            CataloguePageUrl[] results = await ExtractPagination(
                                page,
                                deps.Bypasses,
                                link.Url,
                                logger
                            );

                            await results.PersistMany(session);
                            link = link.MarkProcessed();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Error at extracting pagination for: {Url}", link.Url);
                            link = link.IncreaseRetryCount();
                            browser = await deps.Browsers.Recreate(browser);
                        }
                        finally
                        {
                            links[i] = link;
                        }
                    }
                    
                    await browser.DestroyAsync();
                    await links.UpdateMany(session);

                    Result commit = await txn.Commit(ct);
                    if (commit.IsFailure)
                    {
                        logger.Error(commit.Error, "Error at committing transaction");
                    }
                    
                    logger.Information("Pagination extracting finished.");
                }
                finally
                {
                    BrowserFactory.KillBrowserProcess();
                }
            };
    }

    private static async Task<CataloguePageUrl[]> ExtractPagination(
        IPage page,
        AvitoBypassFactory factory,
        string targetUrl,
        Serilog.ILogger logger
    )
    {
        await page.PerformNavigation(targetUrl);
        IAvitoBypassFirewall bypasser = factory.Create(page);
        if (!await bypasser.Bypass())
            throw new InvalidOperationException("Page is blocked.");
        await page.ScrollBottom();
        const string javaScript = """
            (() => {

            let maxPage = 0;
            const pagedUrls = [];

            const currentUrl = window.location.href;
            const paginationSelector = document.querySelector('nav[aria-label="Пагинация"]');
            if (!paginationSelector) return { maxPage, pagedUrls };

            const paginationGroupSelector = paginationSelector.querySelector('ul[data-marker="pagination-button"]');
            if (!paginationGroupSelector) return { maxPage, pagedUrls };

            const pageNumbersArray = Array.from(
            paginationGroupSelector.querySelectorAll('span[class="styles-module-text-Z0vDE"]'))
                .map(s => parseInt(s.innerText, 10));

            maxPage = Math.max(...pageNumbersArray);

            for (let i = 1; i <= maxPage; i++) {
                const pagedUrl = currentUrl + '&p=' + i;
                pagedUrls.push(pagedUrl);
            }

            return { maxPage, pagedUrls };
            })();
            """;
        PaginationResult result = await page.EvaluateExpressionAsync<PaginationResult>(javaScript);
        CataloguePageUrl[] pages =
        [
            .. Enumerable
                .Range(0, result.MaxPage)
                .Select(p => CataloguePageUrl.New(result.PagedUrls[p])),
        ];
        logger.Information("Extracted {PageCount} pages for url: {Url}", result.MaxPage, targetUrl);
        foreach (CataloguePageUrl pageUrl in pages)
        {
            logger.Information("Paged Url: {PagedUrl}", pageUrl.Url);
        }
        return pages;
    }

    private sealed class PaginationResult
    {
        public int MaxPage { get; set; }
        public string[] PagedUrls { get; set; } = [];
    }
}
