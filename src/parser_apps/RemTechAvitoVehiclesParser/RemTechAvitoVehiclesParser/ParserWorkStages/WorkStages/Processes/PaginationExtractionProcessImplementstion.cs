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
                NpgSqlTransactionSource transactionSource = new(session, logger);
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
                            link.Url
                        );
                        await results.PersistMany(session);
                        link = link.MarkProcessed();
                    }
                    catch (EvaluationFailedException)
                    {
                        browser = await deps.Browsers.Recreate(browser);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error at extracting pagination for: {Url}", link.Url);
                        link = link.IncreaseRetryCount();
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
                    logger.Error(commit.Error, "Error at committing transaction");
                logger.Information("Pagination extracting finished.");
            };
    }

    private static async Task<CataloguePageUrl[]> ExtractPagination(
        IPage page,
        AvitoBypassFactory factory,
        string targetUrl
    )
    {
        await page.PerformNavigation(targetUrl);
        IAvitoBypassFirewall bypasser = factory.Create(page);
        if (!await bypasser.Bypass())
            throw new InvalidOperationException("Page is blocked.");
        await page.ScrollBottom();
        const string javaScript = """
            (() => {
            const currentUrl = window.location.href;
            const paginationSelector = document.querySelector('nav[aria-label="Пагинация"]');
            if (!paginationSelector) return [currentUrl];

            const paginationGroupSelector = paginationSelector.querySelector('ul[data-marker="pagination-button"]');
            if (!paginationGroupSelector) return [currentUrl];

            const pageNumbersArray = Array.from(
            paginationGroupSelector.querySelectorAll('span[class="styles-module-text-Z0vDE"]'))
                .map(s => parseInt(s.innerText, 10));

            const maxPage = Math.max(...pageNumbersArray);
            const pagedUrls = [];

            for (let i = 1; i <= maxPage; i++) {
                const pagedUrl = currentUrl + '&p=' + i;
                pagedUrls.push(pagedUrl);
            }

            return { maxPage, pagedUrls };
            })();
            """;
        PaginationResult result = await page.EvaluateExpressionAsync<PaginationResult>(javaScript);
        return
        [
            .. Enumerable
                .Range(0, result.MaxPage)
                .Select(p => CataloguePageUrl.New(result.PagedUrls[p])),
        ];
    }

    private sealed class PaginationResult
    {
        public int MaxPage { get; set; }
        public string[] PagedUrls { get; set; } = [];
    }
}
