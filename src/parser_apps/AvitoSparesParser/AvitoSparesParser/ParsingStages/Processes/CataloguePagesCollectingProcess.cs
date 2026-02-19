using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using AvitoSparesParser.ParserStartConfiguration;
using AvitoSparesParser.ParserStartConfiguration.Extensions;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParsingStages.Processes;

public static class CataloguePagesCollectingProcess
{
    extension(ParserStageProcess)
    {
        public static ParserStageProcess Pagination =>
            async (deps, stage, session, ct) =>
            {
                deps.Deconstruct(                    
                    out Serilog.ILogger dLogger,
                    out AvitoBypassFactory factory,
                    out BrowserManagerProvider browsers,
                    out _,
                    out _,
                    out _,
                    out ParserStopState stopState
                );                

                Serilog.ILogger logger = dLogger.ForContext<ParserStageProcess>();                                                
                if (stopState.HasStopBeenRequested())
                {
                    await stage.PermanentFinalize(session, ct);
                    return;
                }

                ProcessingParserLink[] links = await GetParserLinksForPagedUrlsExtraction(session, ct);                    
                if (CanSwitchNextStage(links))
                {
                    await SwitchNextStage(stage, session, logger, ct);                    
                    return;
                }

                await using BrowserManager manager = deps.BrowserProvider.Provide();
                await using IPage page = await manager.ProvidePage();
                await ProcessPagedUrlExtraction(links, manager, page, factory, session, logger, stopState);
            };
    }    

    private static async Task<ProcessingParserLink[]> GetParserLinksForPagedUrlsExtraction(NpgSqlSession session, CancellationToken ct)
    {
        ProcessingParserLinkQuery linksQuery = new(
            OnlyNotFetched: true,
            RetryCountThreshold: 5,
            WithLock: true);

        ProcessingParserLink[] links = await IEnumerable<ProcessingParserLink>.QueryMany(
            session,
            linksQuery,
            ct);

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
        Serilog.ILogger logger,
        ParserStopState stopState
    )
    {
        foreach (ProcessingParserLink link in links)
        {
            if (stopState.HasStopBeenRequested())
            {                
                break;
            }

            try
            {
                await ProcessPagedUrlExtractionFromParserLink(
                    link,
                    browserPage,
                    bypassFactory,
                    session,
                    logger);

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

    private static async Task<AvitoCataloguePage[]> ExtractPagination(
        IPage page,
        AvitoBypassFactory factory,
        string targetUrl,
        Serilog.ILogger logger)
    {
        AvitoPagedUrlsExtractor extractor = new(page, factory.Create(page));
        string[] urls = await extractor.ExtractUrls(targetUrl);
        AvitoCataloguePage[] links = [.. urls.Select(AvitoCataloguePage.New)];        
        logger.Information("Extracted {Count} paged urls.", links.Length);
        foreach (var link in links)
        {
            logger.Information("Paged URL: {Url}", link.Url);
        }

        return links;
    }

    private sealed class PaginationResult
    {
        public int MaxPage { get; set; }
        public string[] PagedUrls { get; set; } = [];
    }
}
