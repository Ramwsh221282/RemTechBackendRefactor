using System.Text.Encodings.Web;
using System.Text.Json;
using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.AvitoSpareContext.Extensions;
using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using AvitoSparesParser.ParserStartConfiguration;
using AvitoSparesParser.ParserStartConfiguration.Extensions;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.Parsing;
using ParsingSDK.RabbitMq;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParsingStages.Processes;

public static class ParsingFinalizationProcess
{
    extension(ParserStageProcess)
    {
        public static ParserStageProcess Finalization => async (deps, ct) =>
        {
            NpgSqlConnectionFactory npgSql = deps.NpgSql;
            Serilog.ILogger logger = deps.Logger;
            FinishParserProducer finishProducer = deps.FinishProducer;
            AddContainedItemProducer addProducer = deps.AddContainedItem;
            
            await using NpgSqlSession session = new(npgSql);
            NpgSqlTransactionSource source = new(session, logger);
            await using ITransactionScope scope = await source.BeginTransaction(ct);
            
            Maybe<ParsingStage> finalization = await GetFinalizationStage(session, ct);
            if (!finalization.HasValue) return;
            
            AvitoSpare[] spares = await GetCompletedAvitoSpares(session, ct);
            if (CanSwitchNextStage(spares))
            {
                await FinishParser(session, finishProducer, logger, ct);
                await FinalizeParser(session, logger, ct);
                await FinishTransaction(scope, logger, ct);
                return;
            }
            
            await SendAddContainedItems(session, spares, addProducer, ct);
            await SendResultsToMainBackend(spares, session, logger);
            await FinishTransaction(scope, logger, ct);
        };
    }

    private static async Task SendAddContainedItems(
        NpgSqlSession session, 
        AvitoSpare[] spares, 
        AddContainedItemProducer producer, 
        CancellationToken ct)
    {
        ProcessingParserQuery query = new(WithLock: true);
        ProcessingParser parser = await ProcessingParser.Get(query, session, ct);
        AddContainedItemsMessage message = CreateMessage(parser, spares);
        await producer.Publish(message, ct);
    }

    private static AddContainedItemsMessage CreateMessage(ProcessingParser parser, AvitoSpare[] spares)
    {
        return new AddContainedItemsMessage()
        {
            CreatorDomain = parser.Domain,
            CreatorId = parser.Id,
            CreatorType = parser.Type,
            ItemType = "Запчасти",
            Items = spares.Select(CreatePayload).ToArray(),
        };
    }
    
    private static AddContainedItemsMessagePayload CreatePayload(AvitoSpare spare)
    {
        string id = spare.Id;
        object payload = new
        {
            address = spare.CatalogueRepresentation.Address.Replace('\u00A0', ' '),
            is_nds = spare.CatalogueRepresentation.IsNds,
            oem = spare.CatalogueRepresentation.Oem.Replace('\u00A0', ' '),
            photos = spare.CatalogueRepresentation.Photos.Select(p => p).ToArray(),
            price = spare.CatalogueRepresentation.Price,
            url = spare.CatalogueRepresentation.Url,
            title = spare.ConcreteRepresentation.Title.Replace('\u00A0', ' '),
            type = spare.ConcreteRepresentation.Type.Replace('\u00A0', ' '),
        };

        JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        
        string content = JsonSerializer.Serialize(payload, options);
        return new AddContainedItemsMessagePayload() { ItemId = id, Content = content };
    }
    
    private static async Task FinishParser(NpgSqlSession session, FinishParserProducer finishProducer, Serilog.ILogger logger, CancellationToken ct)
    {
        ProcessingParserQuery query = new(WithLock: true);
        ProcessingParser parser = await ProcessingParser.Get(query, session, ct);
        Guid id = parser.Id;
        long elapsed = parser.Finish().TotalElapsedSeconds();
        await finishProducer.Publish(new FinishParserMessage(id, elapsed), ct);
        logger.Information("Finished parser {Id} in {Elapsed} seconds.", id, elapsed);
    }
    
    private static async Task FinishTransaction(ITransactionScope scope, Serilog.ILogger logger, CancellationToken ct)
    {
        Result commit = await scope.Commit();
        if (commit.IsFailure)
        {
            logger.Error(commit.Error, "Failed to commit transaction.");
        }
    }
    
    private static async Task SendResultsToMainBackend(AvitoSpare[] spares, NpgSqlSession session, Serilog.ILogger logger)
    {
        await spares.RemoveMany(session);
        logger.Information("Sent {Count} results to main backend.", spares.Length);
    }
    
    private static async Task<Maybe<ParsingStage>> GetFinalizationStage(NpgSqlSession session, CancellationToken ct)
    {
        ParsingStageQuery query = new(Name: ParsingStageConstants.FINALIZATION_STAGE, WithLock: true);
        Maybe<ParsingStage> stage = await ParsingStage.GetStage(session, query, ct);
        return stage;        
    }

    private static async Task FinalizeParser(NpgSqlSession session, Serilog.ILogger logger, CancellationToken ct)
    {
        await AvitoSpare.DeleteAll(session, ct);
        await ParsingStage.Delete(session, ct);
        await ProcessingParser.DeleteAllParsers(session, ct);
        await ProcessingParserLink.DeleteAllLinks(session, ct);
        await AvitoCataloguePage.DeleteAll(session, ct);
        logger.Information("Finalized {Stage}");
    }
    
    private static bool CanSwitchNextStage(AvitoSpare[] spares)
    {
        return spares.Length == 0;
    }
    
    private static async Task<AvitoSpare[]> GetCompletedAvitoSpares(NpgSqlSession session, CancellationToken ct)
    {
        AvitoSpareQuery query = new(ConcreteOnly: true, Limit: 50, WithLock: true);
        return await AvitoSpare.Query(session, query, ct);
    }
}