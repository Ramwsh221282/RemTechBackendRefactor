using Dapper;
using ParsingSDK.Parsing;
using RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Models;
using System.Data;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.WorkStages.Extensions;

public static class PostgreSqlWorkStageStorageExtension
{
    extension(ParserWorkStage)
    {        
        public static async Task DeleteAll(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = """
                DELETE FROM avito_parser_module.work_stages;
                """;
            
            CommandDefinition command = new CommandDefinition(sql, cancellationToken: ct, transaction: session.Transaction);
            await session.Execute(command);
        }
        
        public static async Task<Maybe<ParserWorkStage>> GetSingle(
            NpgSqlSession session,
            WorkStageQuery query,
            CancellationToken ct = default
        )
        {
            (DynamicParameters parameters, string filterSql) = query.WhereClause();
            string lockClause = query.LockClause();
            string sql =
            $"""
            SELECT 
            id as id, 
            name as name
            FROM avito_parser_module.work_stages 
            {filterSql}
            {lockClause}
            LIMIT 1
            """;
            CommandDefinition command = session.FormCommand(sql, parameters, ct);
            ParserWorkStage? result = await session.QuerySingleUsingReader(command, reader =>
            {
                Guid id = reader.GetGuid(reader.GetOrdinal("id"));
                string name = reader.GetString(reader.GetOrdinal("name"));
                return ParserWorkStage.Create(id, name);
            });
            
            return result is null ? Maybe<ParserWorkStage>.None() : Maybe<ParserWorkStage>.Some(result);
        }
    }

    extension(ParserWorkStage stage)
    {
        public async Task PermanentFinalize(NpgSqlSession session, CancellationToken ct = default)
        {
            stage.ToFinalizationStage();
            await stage.Update(session, ct);            
        }        

        public async Task Update(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql =
            """
            UPDATE avito_parser_module.work_stages 
            SET name = @name 
            WHERE id = @id
            """;
            object parameters = stage.Parameters;
            CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
            await session.Execute(command);
        }

        public async Task Persist(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql =
            """
            INSERT INTO avito_parser_module.work_stages(id, name)
            VALUES(@id, @name)
            """;
            object parameters = stage.Parameters;
            CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
            await session.Execute(command);
        }

        private object Parameters => new
        {
            id = stage.Id,
            name = stage.Name,
        };
    }

    extension(WorkStageQuery query)
    {
        private (DynamicParameters parameters, string filterSql) WhereClause()
        {
            List<string> filters = [];
            DynamicParameters parameters = new();

            if (query.Id.HasValue)
            {
                filters.Add("id=@id");
                parameters.Add("@id", query.Id.Value, DbType.Guid);
            }

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                filters.Add("name=@name");
                parameters.Add("@name", query.Name, DbType.String);
            }

            string resultSql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
            return (parameters, resultSql);
        }

        private string LockClause() => query.WithLock ? "FOR UPDATE" : string.Empty;
        private string LimitClause() => query.Limit.HasValue ? $"LIMIT {query.Limit}" : string.Empty;
    }
}
