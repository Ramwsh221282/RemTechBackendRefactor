using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
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
                    out BrowserManagerProvider browsers,
                    out _,
                    out _,
                    out _
                );

                Serilog.ILogger logger = dLogger.ForContext<ParserStageProcess>();
                await using NpgSqlSession session = new(npgSql);
                NpgSqlTransactionSource source = new(session);
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

                await using (BrowserManager manager = deps.BrowserProvider.Provide())
                {
                    await using (IPage page = await manager.ProvidePage())
                    {
                        await ProcessPagedUrlExtraction(links, manager, page, factory, session, logger);
                    }
                }
                
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
        BrowserManager manager,
        IPage browserPage,
        AvitoBypassFactory bypassFactory,
        NpgSqlSession session,
        Serilog.ILogger logger
    )
    {
        foreach (ProcessingParserLink link in links)
        {
            try
            {
                await ProcessPagedUrlExtractionFromParserLink(
                    link,
                    browserPage,
                    bypassFactory,
                    session,
                    logger
                );
                link.Marker.MarkProcessed();
            }
            catch(Exception)
            {
                browserPage = await manager.RecreatePage(browserPage);
                manager.ReleasePageUsedMemoryResources();
                link.Counter.Increase();
            }
        }

        await links.UpdateMany(session);
    }

    private static async Task ProcessPagedUrlExtractionFromParserLink(
        ProcessingParserLink link,
        IPage page,
        AvitoBypassFactory bypass,
        NpgSqlSession session,
        Serilog.ILogger logger
    )
    {
        AvitoCataloguePage[] pages = await ExtractPagination(page, bypass, link.Url, logger);
        await pages.AddMany(session);
    }

    private static async Task FinishTransaction(
        ITransactionScope scope,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        Result commit = await scope.Commit(ct);
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Failed to commit transaction.");
        }
    }

    private static async Task<AvitoCataloguePage[]> ExtractPagination(
        IPage page,
        AvitoBypassFactory factory,
        string targetUrl,
        Serilog.ILogger logger
    )
    {
        AvitoPagedUrlsExtractor extractor = new(page, factory.Create(page));
        string[] urls = await extractor.ExtractUrls(targetUrl);
        AvitoCataloguePage[] links = [.. urls.Select(AvitoCataloguePage.New)];        
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
