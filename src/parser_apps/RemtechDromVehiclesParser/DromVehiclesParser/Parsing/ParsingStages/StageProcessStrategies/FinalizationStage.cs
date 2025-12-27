using Dapper;
using DromVehiclesParser.Parsers.Database;
using DromVehiclesParser.Parsers.Models;
using DromVehiclesParser.Parsing.ConcreteItemParsing.Extensions;
using DromVehiclesParser.Parsing.ConcreteItemParsing.Models;
using DromVehiclesParser.Parsing.ParsingStages.Database;
using DromVehiclesParser.Parsing.ParsingStages.Models;
using DromVehiclesParser.ResultsExporing.TextFileExporting;
using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;

public static class FinalizationStage
{
    extension(ParsingStage)
    {
        public static ParsingStage Finalization => async (deps, ct) =>
        {
            deps.Deconstruct(out _, out NpgSqlConnectionFactory npgSql, out Serilog.ILogger dLogger, out IExporter<TextFile> exporter, out FinishParserProducer finishProducer);
            Serilog.ILogger logger = dLogger.ForContext<ParsingStage>();
            await using NpgSqlSession session = new(npgSql);
            NpgSqlTransactionSource transactionSource = new(session);
            ITransactionScope transaction = await transactionSource.BeginTransaction(ct); 
            
            Maybe<ParserWorkStage> stage = await GetFinalizationStage(session);
            if (!stage.HasValue) return;
            
            DromAdvertisementFromPage[] advertisements = await GetAdvertisementsForFinalization(session);
            if (CanSwitchNextStage(advertisements))
            {
                await FinishParser(session, finishProducer, logger, ct);
                await FinalizeParsing(session, logger, ct);
                await FinishTransaction(transaction, logger, ct);
                return;
            }
            
            await FinalizeAdvertisements(advertisements, session, logger, exporter, ct);
            await FinishTransaction(transaction, logger, ct);
        };
    }
    
    private static async Task FinalizeParsing(NpgSqlSession session, Serilog.ILogger logger, CancellationToken ct)
    {
        await RemoveWorkingParserInformation(session, ct);
        await RemoveWorkingParserLinksInformation(session, ct);
        await RemoveCataloguePagesInformation(session, ct);
        await RemoveItemsInformation(session, ct); 
        await ParserWorkStage.ClearTable(session, ct);
        await WorkingParser.DeleteAll(session, ct);
        await WorkingParserLink.DeleteAll(session, ct);
        logger.Information("Parsing finalized");
    }

    private static async Task FinishParser(NpgSqlSession session, FinishParserProducer producer, Serilog.ILogger logger, CancellationToken ct)
    {
        WorkingParserQuery query = new(WithLock: true);
        WorkingParser parser = await WorkingParser.Get(session, query, ct);
        long totalElapsed = parser.Finish().TotalElapsedInSeconds();
        Guid id = parser.Id;
        await producer.Publish(new FinishParserMessage(id, totalElapsed), ct);
        logger.Information("Parser finished: {Id} ({TotalElapsed} seconds)", id, totalElapsed);
    }
    
    private static bool CanSwitchNextStage(DromAdvertisementFromPage[] advertisements)
    {
        return advertisements.Length == 0;
    }
    
    private static async Task FinishTransaction(ITransactionScope transaction, Serilog.ILogger logger, CancellationToken ct)
    {
        Result result = await transaction.Commit(ct);
        if (result.IsFailure)
        {
            logger.Fatal(result.Error, "Failed to commit transaction");
        }
    }
    
    private static async Task FinalizeAdvertisements(
        DromAdvertisementFromPage[] advertisements,
        NpgSqlSession session, 
        Serilog.ILogger logger,
        IExporter<TextFile> exporter,
        CancellationToken ct)
    {
        exporter = exporter.UseLogging(logger);
        string resultDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results");
        Directory.CreateDirectory(resultDirPath);
        foreach (DromAdvertisementFromPage advertisement in advertisements)
        {
            string resultFilePath = Path.Combine(resultDirPath, $"{advertisement.Id}.txt");
            TextFile file = TextFile.FromDromAdvertisement(advertisement, resultFilePath);
            await exporter.Export(file, ct);
        }
        
        await advertisements.RemoveMany(session);
        logger.Information("Advertisements finalized: {Count}", advertisements.Length);
    }
    
    private static async Task<DromAdvertisementFromPage[]> GetAdvertisementsForFinalization(NpgSqlSession session)
    {
        QueryDromAdvertisements query = new(Limit: 50, WithLock: true);
        return await DromAdvertisementFromPage.GetMany(session, query, CancellationToken.None);
    }
    
    private static async Task<Maybe<ParserWorkStage>> GetFinalizationStage(NpgSqlSession session)
    {
        ParserWorkStageStoringImplementation.ParserWorkStageQuery query = new(Name: ParserWorkStageConstants.FINALIZATION, WithLock: true);
        return await ParserWorkStage.FromDb(session, query);
    }

    private static async Task RemoveWorkingParserInformation(NpgSqlSession session, CancellationToken ct)
    {
        const string sql = "DELETE FROM drom_vehicles_parser.working_parsers";
        CommandDefinition command = new(sql, transaction: session.Transaction, cancellationToken: ct);
        await session.Execute(command);
    }

    private static async Task RemoveWorkingParserLinksInformation(NpgSqlSession session, CancellationToken ct)
    {
        const string sql = "DELETE FROM drom_vehicles_parser.working_parser_links";
        CommandDefinition command = new(sql, transaction: session.Transaction, cancellationToken: ct);
        await session.Execute(command);
    }
    
    private static async Task RemoveCataloguePagesInformation(NpgSqlSession session, CancellationToken ct)
    {
        const string sql = "DELETE FROM drom_vehicles_parser.catalogue_pages";
        CommandDefinition command = new(sql, transaction: session.Transaction, cancellationToken: ct);
        await session.Execute(command);
    }

    private static async Task RemoveItemsInformation(NpgSqlSession session, CancellationToken ct)
    {
        const string sql = "DELETE FROM drom_vehicles_parser.items";
        CommandDefinition command = new(sql, transaction: session.Transaction, cancellationToken: ct);
        await session.Execute(command);
    }
}