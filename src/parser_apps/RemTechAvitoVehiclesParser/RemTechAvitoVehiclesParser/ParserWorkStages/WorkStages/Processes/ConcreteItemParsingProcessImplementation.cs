using AvitoFirewallBypass;
using ParsingSDK.Parsing;
using PuppeteerSharp;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractConcreteItem;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Processes;

public static class ConcreteItemParsingProcessImplementation
{
    extension(WorkStageProcess)
    {
        public static WorkStageProcess ConcreteItems =>
            async (deps, ct) =>
            {
                deps.Deconstruct(
                    out BrowserFactory browsers,
                    out AvitoBypassFactory bypasses,
                    out _,
                    out Serilog.ILogger dLogger,
                    out NpgSqlConnectionFactory npgSql,
                    out _,
                    out _
                );

                Serilog.ILogger logger = dLogger.ForContext<WorkStageProcess>();
                await using NpgSqlSession session = new(npgSql);
                NpgSqlTransactionSource transactionSource = new(session);
                await using ITransactionScope txn = await transactionSource.BeginTransaction(ct);

                WorkStageQuery stageQuery = new(
                    Name: WorkStageConstants.ConcreteItemStageName,
                    WithLock: true
                );
                Maybe<ParserWorkStage> workStage = await ParserWorkStage.GetSingle(
                    session,
                    stageQuery,
                    ct
                );
                if (!workStage.HasValue)
                    return;

                AvitoItemQuery itemsQuery = new(
                    UnprocessedOnly: true,
                    Limit: 50,
                    RetryCount: 10,
                    CatalogueOnly: true
                );
                AvitoVehicle[] items = await AvitoVehicle.GetAsCatalogueRepresentation(
                    session,
                    itemsQuery,
                    ct: ct
                );
                if (items.Length == 0)
                {
                    workStage.Value.ToFinalizationStage();
                    await workStage.Value.Update(session, ct);
                    await txn.Commit(ct);
                    logger.Information("Switched to: {Stage}", workStage.Value.Name);
                    return;
                }

                IBrowser browser = await browsers.ProvideBrowser();

                for (int i = 0; i < items.Length; i++)
                {
                    AvitoVehicle item = items[i];
                    logger.Information("Processing item: {Url}", item.CatalogueRepresentation.Url);

                    try
                    {
                        item = await new ExtractConcreteItemCommand(
                            () => browser.GetPage(),
                            item,
                            bypasses
                        )
                            .UseLogging(dLogger)
                            .Handle();
                        item = item.MarkProcessed();
                    }
                    catch (EvaluationFailedException)
                    {
                        logger.Error(
                            "Evaluation failed for item {Url}. Recreating browser",
                            item.CatalogueRepresentation.Url
                        );
                        browser = await browsers.Recreate(browser);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(
                            ex,
                            "Error at extracting concrete item {Url}.",
                            item.CatalogueRepresentation.Url
                        );
                        item = item.IncreaseRetryCount();
                    }
                    finally
                    {
                        items[i] = item;
                    }
                }

                await browser.DestroyAsync();
                await items.UpdateFull(session);

                Result commit = await txn.Commit(ct);
                if (commit.IsFailure)
                    logger.Error(commit.Error, "Error at committing transaction");
            };
    }
}
