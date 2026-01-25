using System.Data;
using Dapper;
using ParsingSDK.Parsing;
using RemTech.SharedKernel.Infrastructure.Database;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing.Extensions;

public sealed record ProcessingParserQuery(Guid? Id = null, bool WithLock = false);

public static class ProcessingParserStoringImplementation
{
    extension(ProcessingParser)
    {
        public static async Task<bool> HasAny(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = """
                SELECT COUNT(*) FROM avito_parser_module.parsers;
                """;
            CommandDefinition command = new(
                sql,
                cancellationToken: ct,
                transaction: session.Transaction
            );
            long count = await session.QuerySingleRow<long>(command);
            return count > 0;
        }

        public static async Task DeleteParser(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = """
                DELETE FROM avito_parser_module.parsers;
                """;
            
            CommandDefinition command = new CommandDefinition(sql, cancellationToken: ct, transaction: session.Transaction);
            await session.Execute(command);
        }

        public static async Task<Maybe<ProcessingParser>> Get(NpgSqlSession session, ProcessingParserQuery query, CancellationToken ct = default)
        {
            (DynamicParameters parameters, string filterSql) = WhereClause(query);
            string lockClause = LockClause(query);
            string sql = $"""
                SELECT 
                id as id, 
                domain as domain, 
                type as type,
                start_datetime as start_datetime,
                end_datetime as end_datetime
                FROM avito_parser_module.parsers
                {filterSql}
                {lockClause}
                """;
            
            CommandDefinition command = session.FormCommand(sql, parameters, ct);
            ProcessingParser? parser = await session.QuerySingleUsingReader(command, reader =>
            {
                Guid id = reader.GetValue<Guid>("id");
                string domain = reader.GetValue<string>("domain");
                string type = reader.GetValue<string>("type");
                DateTime startDateTime = reader.GetNullable<DateTime>("start_datetime")!.Value;
                DateTime? endDateTime = reader.GetNullable<DateTime>("end_datetime");
                return new ProcessingParser(id, domain, type, startDateTime, endDateTime);
            });
            
            return parser is null ? Maybe<ProcessingParser>.None() : Maybe<ProcessingParser>.Some(parser);
        }
    }

    extension(ProcessingParser parser)
    {
        public async Task Persist(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = """
                INSERT INTO avito_parser_module.parsers
                (id, domain, type, start_datetime, end_datetime)
                VALUES
                (@id, @domain, @type, @start_datetime, @end_datetime)
                """;
            
            CommandDefinition command = new CommandDefinition(sql, parser.ExtractParameters(), transaction: session.Transaction, cancellationToken: ct);
            await session.Execute(command);
        }

        private object ExtractParameters() =>
            new
            {
                id = parser.Id,
                domain = parser.Domain,
                type = parser.Type,
                start_datetime = parser.StartDateTime,
                end_datetime = parser.EndDateTime.HasValue ? parser.EndDateTime.Value : (DateTime?)null,
            };
    }

    extension(ProcessingParserQuery query)
    {
        private (DynamicParameters, string filters) WhereClause()
        {
            List<string> filters = [];
            DynamicParameters parameters = new();

            if (query.Id.HasValue)
            {
                filters.Add("id=@id");
                parameters.Add("@id", query.Id.Value, DbType.Guid);
            }

            return filters.Count == 0 ? (parameters, string.Empty) : (parameters, $"WHERE {string.Join(" AND ", filters)}");
        }
        
        private string LockClause() => query.WithLock ? "FOR UPDATE" : string.Empty;
    }
}
