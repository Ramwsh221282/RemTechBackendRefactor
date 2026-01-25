using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using AvitoSparesParser.Commands.ExtractPagedUrls;
using AvitoSparesParser.ParserStartConfiguration;
using AvitoSparesParser.ParserStartConfiguration.Extensions;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParsingStages.Processes;

public static class CataloguePagesCollectingProcess
{
    extension(ParserStageProcess)
    {
        public static ParserStageProcess Pagination =>
            async (deps, ct) =>
            {
                deps.Deconstruct(
                    out NpgSqlConnectionFactory npgSql,
                    out Serilog.ILogger dLogger,
                    out AvitoBypassFactory factory,
                    out BrowserFactory browsers,
                    out _,
                    out _,
                    out _
                );

                Serilog.ILogger logger = dLogger.ForContext<ParserStageProcess>();
                await using NpgSqlSession session = new(npgSql);
                NpgSqlTransactionSource source = new(session, logger);
                await using ITransactionScope scope = await source.BeginTransaction(ct);

                Maybe<ParsingStage> stage = await GetPaginationStage(session, ct);
                if (!stage.HasValue)
                    return;

                ProcessingParserLink[] links = await GetParserLinksForPagedUrlsExtraction(
                    session,
                    ct
                );
                if (CanSwitchNextStage(links))
                {
                    await SwitchNextStage(stage.Value, session, logger, ct);
                    await FinishTransaction(scope, logger, ct);
                    return;
                }

                await ProcessPagedUrlExtraction(links, browsers, factory, session, logger);
                await FinishTransaction(scope, logger, ct);
            };
    }

    private static async Task<Maybe<ParsingStage>> GetPaginationStage(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        ParsingStageQuery stageQuery = new(Name: ParsingStageConstants.PAGINATION, WithLock: true);
        Maybe<ParsingStage> stage = await ParsingStage.GetStage(session, stageQuery, ct);
        return stage;
    }

    private static async Task<ProcessingParserLink[]> GetParserLinksForPagedUrlsExtraction(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        ProcessingParserLinkQuery linksQuery = new(
            OnlyNotFetched: true,
            RetryCountThreshold: 5,
            WithLock: true
        );
        ProcessingParserLink[] links = await IEnumerable<ProcessingParserLink>.QueryMany(
            session,
            linksQuery,
            ct
        );
        return links;
    }

    private static bool CanSwitchNextStage(ProcessingParserLink[] links)
    {
        return links.Length == 0;
    }

    private static async Task SwitchNextStage(
        ParsingStage stage,
        NpgSqlSession session,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        ParsingStage catalogueStage = stage.ToCatalogueStage();
        await catalogueStage.Update(session, ct);
        logger.Information("Switched to: {Stage}", catalogueStage.Name);
    }

    private static async Task ProcessPagedUrlExtraction(
        ProcessingParserLink[] links,
        BrowserFactory browsers,
        AvitoBypassFactory bypassFactory,
        NpgSqlSession session
    )
    {
        IBrowser browser = await browsers.ProvideBrowser();

        for (int i = 0; i < links.Length; i++)
        {
            ProcessingParserLink link = links[i];
            IPage page = await browser.GetPage();
            links[i] = await ProcessPagedUrlExtractionFromParserLink(
                link,
                page,
                bypassFactory,
                session
            );
        }

        await links.UpdateMany(session);
        await browser.DestroyAsync();
    }

    private static async Task<ProcessingParserLink> ProcessPagedUrlExtractionFromParserLink(
        ProcessingParserLink link,
        IPage page,
        AvitoBypassFactory bypass,
        NpgSqlSession session
    )
    {
        try
        {
            AvitoCataloguePage[] pages = await ExtractPagination(page, bypass, link.Url);
            await pages.AddMany(session);
            link.Marker.MarkProcessed();
        }
        catch (Exception)
        {
            link.Counter.Increase();
        }

        return link;
    }

    private static async Task FinishTransaction(
        ITransactionScope scope,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        Result commit = await scope.Commit();
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Failed to commit transaction.");
        }
    }

    // todo move this into shared avito library.
    private static async Task<AvitoCataloguePage[]> ExtractPagination(
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
        AvitoCataloguePage[] links =
        [
            .. Enumerable
                .Range(0, result.MaxPage)
                .Select(p => AvitoCataloguePage.New(result.PagedUrls[p])),
        ];
        logger.Information("Extracted {Count} paged urls.", links.Length);
        foreach (var link in links)
            logger.Information("Paged URL: {Url}", link.Url);
        return links;
    }

    private sealed class PaginationResult
    {
        public int MaxPage { get; set; }
        public string[] PagedUrls { get; set; } = [];
    }
}
