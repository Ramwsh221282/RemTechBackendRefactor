using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;
using AvitoFirewallBypass;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractCatalogueItemData;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Processes;

public static class CataloguePagesParsingProcessImplementation
{
    extension(WorkStageProcess)
    {
        public static WorkStageProcess CatalogueProcess =>
            async (deps, ct) =>
            {
                deps.Deconstruct(
                    out BrowserFactory browsers,
                    out AvitoBypassFactory bypasses,
                    out _,
                    out Serilog.ILogger dLogger,
                    out NpgSqlConnectionFactory npgSql,
                    out _
                );

                Serilog.ILogger logger = dLogger.ForContext<WorkStageProcess>();
                await using NpgSqlSession session = new(npgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                ITransactionScope txn = await transactionSource.BeginTransaction(ct);

                WorkStageQuery stageQuery = new(Name: WorkStageConstants.CatalogueStageName, WithLock: true);
                Maybe<ParserWorkStage> stage = await ParserWorkStage.GetSingle(session, stageQuery, ct);
                if (!stage.HasValue) return;

                CataloguePageUrlQuery pageUrlQuery = new(
                    UnprocessedOnly: true,
                    RetryLimit: 10,
                    WithLock: true,
                    Limit: 20
                );
                CataloguePageUrl[] urls = await CataloguePageUrl.GetMany(session, pageUrlQuery, ct);
                if (urls.Length == 0)
                {
                    stage.Value.ToConcreteStage();
                    await stage.Value.Update(session, ct);
                    await txn.Commit(ct);
                    logger.Information("Switched to stage: {Stage}", stage.Value.Name);
                    return;
                }

                IBrowser browser = await deps.Browsers.ProvideBrowser();

                for (int i = 0; i < urls.Length; i++)
                {
                    CataloguePageUrl url = urls[i];
                    logger.Information("Processing page: {Url}", url.Url);

                    try
                    {
                        AvitoVehicle[] items = await new ExtractCatalogueItemDataCommand(() => browser.GetPage(), url, bypasses).UseLogging(dLogger).Handle();
                        url = url.MarkProcessed();
                        await items.PersistAsCatalogueRepresentation(session);
                    }
                    catch(EvaluationFailedException)
                    {
                        browser = await browsers.Recreate(browser);
                    }
                    catch (Exception)
                    {
                        url = url.IncrementRetryCount();
                    }
                    finally
                    {
                        urls[i] = url;
                    }
                }

                await browser.DestroyAsync();
                await urls.UpdateMany(session);
                Result commit = await txn.Commit(ct);
                if (commit.IsFailure) logger.Error(commit.Error, "Error at committing transaction");
            };
    }
}
