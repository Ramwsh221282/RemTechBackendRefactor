using System.Data;
using Dapper;
using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParserStartConfiguration.Extensions;

public sealed record ProcessingParserQuery(Guid? Id = null, bool WithLock = false);

public static class ProcessingParserStoringExtensions
{
    extension(ProcessingParserLink)
    {
        public static async Task DeleteAllLinks(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = "DELETE FROM avito_spares_parser.processing_parser_links";
            CommandDefinition command = new(sql, cancellationToken: ct, transaction: session.Transaction);
            await session.Execute(command);
        }
    }
    
    extension(ProcessingParser)
    {
        public static async Task DeleteAllParsers(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = "DELETE FROM avito_spares_parser.processing_parsers";
            CommandDefinition command = new(sql, cancellationToken: ct, transaction: session.Transaction);
            await session.Execute(command);
        }

        public static async Task<ProcessingParser> Get(ProcessingParserQuery query, NpgSqlSession session, CancellationToken ct = default)
        {
            (DynamicParameters parameters, string filterSql) = WhereClause(query);
            string lockClause = LockClause(query);
            string sql = $"""
            SELECT
            id as id,
            domain as domain,
            type as type,
            finished as finished,
            entered as entered
            FROM avito_spares_parser.processing_parsers
            {filterSql}
            {lockClause}
            """;
            CommandDefinition command = session.FormCommand(sql, parameters, ct: ct);
            ProcessingParser? parser = await session.QuerySingleUsingReader(command, reader =>
            {
                Guid id = reader.GetValue<Guid>("id");
                string domain = reader.GetValue<string>("domain");
                string type = reader.GetValue<string>("type");
                DateTime entered = reader.GetValue<DateTime>("entered");
                DateTime? finished = reader.GetNullable<DateTime>("finished");
                return new ProcessingParser(id, domain, type, entered, finished);
            });
            
            return parser ?? throw new InvalidOperationException($"Parser not found for query {query}");
        }
        
        public static async Task<bool> Exists(NpgSqlSession session)
        {
            const string sql = "SELECT EXISTS (SELECT 1 FROM avito_spares_parser.processing_parsers)";
            CommandDefinition command = new(sql, transaction: session.Transaction);
            bool exists = await session.QuerySingleRow<bool>(command);
            return exists;
        }
    }
    
    extension(ProcessingParserLinkQuery query)
    {
        private (DynamicParameters parameters, string filterSql) WhereClause()
        {
            List<string> filters = [];
            DynamicParameters parameters = new();

            if (query.RetryCountThreshold.HasValue)
            {
                filters.Add("retry_count <= @retryThreshold");
                parameters.Add("@retryThreshold", query.RetryCountThreshold.Value, DbType.Int32);
            }

            if (query.OnlyFetched) filters.Add("processed is true");
            if (query.OnlyNotFetched) filters.Add("processed is false");

            return filters.Count == 0 ? (parameters, string.Empty) : (parameters, "WHERE " + string.Join(" AND ", filters));
        }

        private string LockClause() => query.WithLock ? "FOR UPDATE" : string.Empty;
    }

    extension(IEnumerable<ProcessingParserLink>)
    {
        public static async Task<ProcessingParserLink[]> QueryMany(
            NpgSqlSession session,
            ProcessingParserLinkQuery query,
            CancellationToken ct = default
        )
        {
            (DynamicParameters parameters, string filterSql) = WhereClause(query);
            string lockClause = query.LockClause();
            string sql = $"""
            SELECT
            id as id,
            url as url,
            processed as processed,
            retry_count as retry_count
            FROM avito_spares_parser.processing_parser_links
            {filterSql}
            {lockClause}
            """;
            CommandDefinition command = session.FormCommand(sql, parameters, ct: ct);
            return await session.QueryMultipleUsingReader(command, CreateBy);
        }
    }

    extension(IEnumerable<ProcessingParserLink> links)
    {
        public async Task UpdateMany(NpgSqlSession session)
        {
            const string sql = """
            UPDATE avito_spares_parser.processing_parser_links
            SET
                processed = @processed,
                retry_count = @retry_count
            WHERE id = @id
            """;
            object[] parameters = links.Select(link => link.ExtractParameters()).ToArray();
            await session.ExecuteBulk(sql, parameters);
        }

        public async Task AddMany(NpgSqlSession session)
        {
            const string sql = """
            INSERT INTO avito_spares_parser.processing_parser_links
            (id, url, processed, retry_count)
            VALUES
            (@id, @url, @processed, @retry_count)
            """;
            object[] parameters = links.Select(link => link.ExtractParameters()).ToArray();
            await session.ExecuteBulk(sql, parameters);
        }
    }

    extension(ProcessingParser parser)
    {
        public async Task Add(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = """
            INSERT INTO avito_spares_parser.processing_parsers
            (id, domain, type, finished, entered)
            VALUES
            (@id, @domain, @type, @finished, @entered)
            """;

            CommandDefinition command = new(sql, parser.ExtractParameters(), transaction: session.Transaction, cancellationToken: ct);
            await session.Execute(command);
        }

        private object ExtractParameters() => new
        {
            id = parser.Id,
            domain = parser.Domain,
            type = parser.Type,
            finished = parser.Finished,
            entered = parser.Entered,
        };
    }

    extension(ProcessingParserLink link)
    {
        private object ExtractParameters() => new
        {
            id = link.Id,
            url = link.Url,
            processed = link.Marker.Processed,
            retry_count = link.Counter.Value,
        };
    }

    private static ProcessingParserLink CreateBy(IDataReader reader)
    {
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string url = reader.GetString(reader.GetOrdinal("url"));
        bool processed = reader.GetBoolean(reader.GetOrdinal("processed"));
        int retry_count = reader.GetInt32(reader.GetOrdinal("retry_count"));
        return new ProcessingParserLink(
            id,
            url,
            new Common.RetryCounter(retry_count),
            new Common.ProcessedMarker(processed)
        );
    }

    private static (DynamicParameters parameters, string filterSql) WhereClause(ProcessingParserQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();

        if (query.Id.HasValue)
        {
            filters.Add("id = @id");
            parameters.Add("id", query.Id.Value);
        }

        return filters.Count == 0 ? (parameters, string.Empty) : (parameters, "WHERE " + string.Join(" AND ", filters));
    }

    private static string LockClause(ProcessingParserQuery query)
    {
        return query.WithLock ? "FOR UPDATE" : string.Empty;
    }
}
