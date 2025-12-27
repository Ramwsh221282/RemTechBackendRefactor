using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common;
using RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Processes;

public static class FinalizationWorkStageProcessImplementation
{
    extension(WorkStageProcess)
    {
        public static WorkStageProcess Finalization => async (deps, ct) =>
        {
             deps.Deconstruct(
                 out _, out _, out _, 
                 out Serilog.ILogger dLogger, 
                 out NpgSqlConnectionFactory npgSql,
                 out FinishParserProducer finishProducer);

            Serilog.ILogger logger = dLogger.ForContext<WorkStageProcess>();
            await using NpgSqlSession session = new(npgSql);
            NpgSqlTransactionSource transactionSource = new(session);
            ITransactionScope txn = await transactionSource.BeginTransaction(ct);
            
            Maybe<ParserWorkStage> stage = await GetFinalizationStage(session, logger, ct);
            if (!stage.HasValue) return;
            
            AvitoVehicle[] items = await GetFinalizationItems(session, logger, ct);
            if (CanFinalize(items))
            {
                await FinalizeParser(session, finishProducer, logger, ct);
                await FinalizeStage(session, logger, ct);
                await FinishTransaction(txn, logger, ct);
                return;
            }

            await items.Remove(session);
            logger.Information("Finalized {Count} items.", items.Length);
            await FinishTransaction(txn, logger, ct);
        };
    }

    private static bool CanFinalize(AvitoVehicle[] items)
    {
        return items.Length == 0;
    }

    private static async Task FinishTransaction(ITransactionScope txn, Serilog.ILogger logger, CancellationToken ct)
    {
        Result result = await txn.Commit(ct);
        if (result.IsFailure) logger.Error(result.Error, "Error at committing transaction");
        else logger.Information("Transaction committed successfully.");
    }
    
    private static async Task<AvitoVehicle[]> GetFinalizationItems(NpgSqlSession session, Serilog.ILogger logger, CancellationToken ct)
    {
        AvitoItemQuery itemsQuery = new(ConcreteOnly: true, WithLock: true, Limit: 50);
        AvitoVehicle[] items = await AvitoVehicle.GetAsConcreteRepresentation(session, itemsQuery, ct);
        logger.Information("Found {Count} items for finalization.", items.Length);
        return items;
    }
    
    private static async Task<Maybe<ParserWorkStage>> GetFinalizationStage(NpgSqlSession session, Serilog.ILogger logger, CancellationToken ct)
    {
        WorkStageQuery stageQuery = new(Name: WorkStageConstants.FinalizationStage, WithLock: true);
        Maybe<ParserWorkStage> stage = await ParserWorkStage.GetSingle(session, stageQuery, ct);
        if (!stage.HasValue) logger.Warning("No finalization stage found.");
        return stage;
    }
    
    private static async Task FinalizeParser(NpgSqlSession session, FinishParserProducer producer, Serilog.ILogger logger, CancellationToken ct)
    {
        ProcessingParserQuery query = new(WithLock: true);
        Maybe<ProcessingParser> parser = await ProcessingParser.Get(session, query, ct);
        if (!parser.HasValue) throw new InvalidOperationException("No active parser found.");
        long elapsed = parser.Value.Finish().CalculateTotalElapsedSeconds();
        Guid id = parser.Value.Id;
        await producer.Publish(new FinishParserMessage(id, elapsed), ct);
        logger.Information("Parser finalized in {Elapsed} seconds.", elapsed);
    }

    private static async Task FinalizeStage(NpgSqlSession session, Serilog.ILogger logger, CancellationToken ct)
    {
        logger.Information("Finalizing stage...");
        await CataloguePageUrl.DeleteAll(session, ct);
        await ProcessingParser.DeleteParser(session, ct);
        await ProcessingParserLink.DeleteAll(session, ct);
        await ParserWorkStage.DeleteAll(session, ct);
        await AvitoVehicle.DeleteAll(session, ct);
        logger.Information("Stage finalized.");
    }
}