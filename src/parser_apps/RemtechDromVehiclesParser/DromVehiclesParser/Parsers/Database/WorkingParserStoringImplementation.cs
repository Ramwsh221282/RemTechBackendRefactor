using System.Data;
using Dapper;
using DromVehiclesParser.Parsers.Models;
using RemTech.SharedKernel.Infrastructure.Database;

namespace DromVehiclesParser.Parsers.Database;

public sealed record WorkingParserQuery(Guid? Id = null, bool WithLock = false);

public static class WorkingParserStoringImplementation
{
    extension(WorkingParser)
    {
        public static async Task DeleteAll(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = "DELETE FROM drom_vehicles_parser.working_parsers";
            CommandDefinition command = session.FormCommand(sql, null, ct);
            await session.Execute(command);
        }
        
        public static async Task<bool> Exists(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = """
                               SELECT EXISTS(
                                   SELECT 1 
                                   FROM drom_vehicles_parser.working_parsers
                               )
                               """;
            CommandDefinition command = session.FormCommand(sql, null, ct);
            return await session.QuerySingleRow<bool>(command);
        }

        public static async Task<WorkingParser> Get(NpgSqlSession session, WorkingParserQuery query, CancellationToken ct = default)
        {
            (DynamicParameters parameters, string filterSql) = WhereClause(query);
            string lockClause = LockClause(query);
            string sql = $"""
                               SELECT 
                                   id as id, 
                                   domain as domain, 
                                   type as type, 
                                   start_date_time as start_date_time, 
                                   end_date_time as end_date_time 
                               FROM drom_vehicles_parser.working_parsers
                               {filterSql}
                               {lockClause}
                               """;
            
            CommandDefinition command = session.FormCommand(sql, parameters, ct);
            WorkingParser? parser = await session.QuerySingleUsingReader(command, reader =>
            {
                Guid id = reader.GetValue<Guid>("id");
                string domain = reader.GetValue<string>("domain");
                string type = reader.GetValue<string>("type");
                DateTime startDateTime = reader.GetNullable<DateTime>("start_date_time")!.Value;
                DateTime? endDateTime = reader.GetNullable<DateTime>("end_date_time");
                return new WorkingParser(id, domain, type, startDateTime, endDateTime);
            });
            
            return parser ?? throw new InvalidOperationException("No working parser found");
        }
    }
    
    extension(WorkingParser parser)
    {
        public async Task Persist(NpgSqlSession session, CancellationToken ct)
        {
            const string sql = """
                               INSERT INTO drom_vehicles_parser.working_parsers
                               (id, domain, type, start_date_time, end_date_time)
                               VALUES
                               (@id, @domain, @type, @start_date_time, @end_date_time)
                               ON CONFLICT (id) DO NOTHING
                               """;
            CommandDefinition command = session.FormCommand(sql, parser.ExtractParameters(), ct);
            await session.Execute(command);
        }

        private object ExtractParameters() => new
        {
            id = parser.Id,
            domain = parser.Domain,
            type = parser.Type,
            start_date_time = parser.StartDateTime,
            end_date_time = parser.EndDateTime.HasValue ? parser.EndDateTime.Value : (DateTime?)null
        };
    }

    private static (DynamicParameters, string) WhereClause(WorkingParserQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        
        if (query.Id.HasValue)
        {
            filters.Add("id = @id");
            parameters.Add("@id", query.Id.Value, DbType.Guid);
        }

        return filters.Count == 0 ? (parameters, string.Empty) : (parameters, "WHERE " + string.Join(" AND ", filters));
    }
    
    private static string LockClause(WorkingParserQuery query)
    {
        return query.WithLock ? "FOR UPDATE" : string.Empty;
    }
}