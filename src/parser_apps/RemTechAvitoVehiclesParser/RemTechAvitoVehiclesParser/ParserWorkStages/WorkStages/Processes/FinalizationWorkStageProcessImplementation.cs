using ParsingSDK.Parsing;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTechAvitoVehiclesParser.ParserWorkStages.Common;
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
                 out _, 
                 out _, 
                 out _, 
                 out Serilog.ILogger dLogger, 
                 out NpgSqlConnectionFactory npgSql);

            Serilog.ILogger logger = dLogger.ForContext<WorkStageProcess>();
            await using NpgSqlSession session = new(npgSql);
            NpgSqlTransactionSource transactionSource = new(session);
            ITransactionScope txn = await transactionSource.BeginTransaction(ct);

            WorkStageQuery stageQuery = new(Name: WorkStageConstants.FinalizationStage, WithLock: true);
            Maybe<ParserWorkStage> stage = await ParserWorkStage.GetSingle(session, stageQuery, ct);
            if (!stage.HasValue) return;

            AvitoItemQuery itemsQuery = new(ConcreteOnly: true, WithLock: true, Limit: 50);
            AvitoVehicle[] items = await AvitoVehicle.GetAsConcreteRepresentation(session, itemsQuery, ct);
            
            if (items.Length == 0)
            {
                stage.Value.ToSleepingStage();
                await stage.Value.Update(session, ct);
                await txn.Commit(ct);
                logger.Information("Switched to sleeping work stage.");
                return;
            }

            await items.Remove(session);
            logger.Information("Finalized {Count} items.", items.Length);
            
            Result result = await txn.Commit(ct);
            if (result.IsFailure) logger.Error(result.Error, "Error at committing transaction");
        };
    }
}