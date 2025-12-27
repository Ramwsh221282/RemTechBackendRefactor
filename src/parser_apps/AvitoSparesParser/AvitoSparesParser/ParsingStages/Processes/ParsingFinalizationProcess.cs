using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.AvitoSpareContext.Extensions;
using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using AvitoSparesParser.ParserStartConfiguration;
using AvitoSparesParser.ParserStartConfiguration.Extensions;
using AvitoSparesParser.ParsingStages.Extensions;
using ParsingSDK.Parsing;
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
            deps.Deconstruct(out NpgSqlConnectionFactory npgSql, out Serilog.ILogger logger, out _, out _, out _);
            await using NpgSqlSession session = new(npgSql);
            NpgSqlTransactionSource source = new(session);
            ITransactionScope scope = await source.BeginTransaction(ct);
            
            Maybe<ParsingStage> finalization = await GetFinalizationStage(session, ct);
            if (!finalization.HasValue) return;
            
            AvitoSpare[] spares = await GetCompletedAvitoSpares(session, ct);
            if (CanSwitchNextStage(spares))
            {
                await FinalizeParser(session, deps.Logger, ct);
                await FinishTransaction(scope, deps.Logger, ct);
                return;
            }
            
            await SendResultsToMainBackend(spares, session, logger);
            await FinishTransaction(scope, logger, ct);
        };
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