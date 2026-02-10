using DromVehiclesParser.Commands.ExtractPagedUrls;
using DromVehiclesParser.Parsers.Database;
using DromVehiclesParser.Parsers.Models;
using DromVehiclesParser.Parsing.CatalogueParsing.Extensions;
using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public static class PagedUrlsExtractingStage
{
    extension(ParsingStage)
    {
        public static ParsingStage Pagination =>
            async (deps, ct) =>
            {
                Serilog.ILogger logger = deps.Logger;
                await using NpgSqlSession session = new(deps.NpgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope transaction = await transactionSource.BeginTransaction(ct);

                Maybe<ParserWorkStage> stage = await GetPaginationStage(session, ct);
                if (StageIsNotPaginationStage(stage))
                {
                    return;
                }

                WorkingParserLink[] links = await GetParserLinksForPaginationUrlsExtraction(session, ct);
                if (CanSwitchNextStage(links))
                {
                    await SwitchNextStage(stage, session, logger, ct);
                    await FinishTransaction(transaction, logger, ct);
                    return;
                }

                await using BrowserManager manager = deps.Browsers.Provide();
                await using IPage page = await manager.ProvidePage();
                
                await CollectPagedUrlsFromLinks(links, session, manager, page, logger);
                await FinishTransaction(transaction, logger, ct);
            };
    }

    private static async Task CollectPagedUrlsFromLinks(
        WorkingParserLink[] links,
        NpgSqlSession session,
        BrowserManager manager,
        IPage page,
        Serilog.ILogger logger
    )
    {
        for (int i = 0; i < links.Length; i++)
        {
            WorkingParserLink link = links[i];
            
            links[i] = await CollectPagedUrlsFromLink(
                manager,
                page,
                link,
                session,
                logger
            );
            
            await Task.Delay(TimeSpan.FromSeconds(5)); // forced delay to avoid get blocked.
        }

        await links.UpdateMany(session);
    }

    private static async Task<WorkingParserLink> CollectPagedUrlsFromLink(
        BrowserManager manager,
        IPage page,
        WorkingParserLink link,
        NpgSqlSession session,
        Serilog.ILogger logger
    )
    {
        IExtractPagedUrlsCommand command = new ExtractPagedUrlsCommand(page).UseLogging(logger);
        
        try
        {
            DromCataloguePage[] pages = [.. await command.Extract(link.Url)];
            await pages.PersistMany(session);
            logger.Information("Collected pages for link: {Url}", link.Url);
            return link.MarkProcessed();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to extract pages for link: {Url}", link.Url);
            page = await manager.RecreatePage(page);
            manager.ReleasePageUsedMemoryResources();
            return link.IncreaseRetryCount();
        }
    }

    private static async Task FinishTransaction(
        ITransactionScope transaction,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        Result result = await transaction.Commit(ct);
        if (result.IsFailure)
        {
            logger.Fatal(result.Error, "Failed to commit transaction");
        }
    }

    private static async Task SwitchNextStage(
        Maybe<ParserWorkStage> stage,
        NpgSqlSession session,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        ParserWorkStage catalogueStage = stage.Value.CatalogueStage();
        await catalogueStage.Update(session, ct);
        logger.Information("Switched to stage: {Name}", catalogueStage.StageName);
    }

    private static bool CanSwitchNextStage(WorkingParserLink[] links)
    {
        return links.Length == 0;
    }

    private static async Task<WorkingParserLink[]> GetParserLinksForPaginationUrlsExtraction(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        WorkingParserLinkQuery query = new(UnprocessedOnly: true, RetryLimit: 5, WithLock: true);
        WorkingParserLink[] links = await WorkingParserLink.GetMany(session, query, ct);
        return links;
    }

    private static async Task<Maybe<ParserWorkStage>> GetPaginationStage(
        NpgSqlSession session,
        CancellationToken ct
    )
    {
        ParserWorkStageStoringImplementation.ParserWorkStageQuery query = new(
            Name: ParserWorkStageConstants.PAGINATION,
            WithLock: true
        );
        Maybe<ParserWorkStage> stage = await ParserWorkStage.FromDb(session, query, ct);
        return stage;
    }

    private static bool StageIsNotPaginationStage(Maybe<ParserWorkStage> stage)
    {
        return !stage.HasValue;
    }
}
