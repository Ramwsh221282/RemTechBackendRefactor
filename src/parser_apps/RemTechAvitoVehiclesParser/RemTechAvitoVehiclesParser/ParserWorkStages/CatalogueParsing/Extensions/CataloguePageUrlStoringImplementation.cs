using System.Data;
using Dapper;
using RemTech.SharedKernel.Infrastructure.Database;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.CatalogueParsing.Extensions;

public static class CataloguePageUrlStoringImplementation
{
    extension(CataloguePageUrl)
    {
        public static async Task DeleteAll(NpgSqlSession session, CancellationToken ct = default)
        {
            const string sql = """
                DELETE FROM avito_parser_module.catalogue_urls;
                """;
            
            CommandDefinition command = new CommandDefinition(sql, cancellationToken: ct, transaction: session.Transaction);
            await session.Execute(command);
        }
        
        public static async Task<CataloguePageUrl[]> GetMany(
            NpgSqlSession session,
            CataloguePageUrlQuery query,
            CancellationToken ct = default
        )
        {
            (DynamicParameters parameters, string filterSql) = query.WhereClause();
            string lockClause = query.LockClause();
            string limitClause = query.LimitClause();
            string sql = $"""
                SELECT url, was_processed, retry_count
                FROM avito_parser_module.catalogue_urls
                {filterSql}
                {lockClause}
                {limitClause}
                """;
            
            CommandDefinition command = session.FormCommand(sql, parameters, ct);
            return await session.QueryMultipleUsingReader(command, r =>
            {
                string url = r.GetString(r.GetOrdinal("url"));
                bool processed = r.GetBoolean(r.GetOrdinal("was_processed"));
                int retryCount = r.GetInt32(r.GetOrdinal("retry_count"));
                return new CataloguePageUrl(url, processed, retryCount);
            });
        }
    }

    extension(IEnumerable<CataloguePageUrl> urls)
    {
        public async Task UpdateMany(NpgSqlSession session)
        {
            const string sql = """
                UPDATE avito_parser_module.catalogue_urls
                SET was_processed = @processed,
                    retry_count = @retry_count
                WHERE url = @url;
                """;
            
            object[] parameters = [..urls.Select(l => l.ExtractParameters())];
            await session.ExecuteBulk(sql, parameters);
        }

        public async Task PersistMany(NpgSqlSession session)
        {
            const string sql = """
                INSERT INTO avito_parser_module.catalogue_urls
                (url, was_processed, retry_count)
                VALUES
                (@url, @processed, @retry_count) ON CONFLICT (url) DO NOTHING;
                """;
            
            object[] parameters = [..urls.Select(l => l.ExtractParameters())];
            await session.ExecuteBulk(sql, parameters);
        }
    }

    extension(CataloguePageUrl url)
    {
        private object ExtractParameters() =>
            new
            {
                url = url.Url,
                processed = url.Processed,
                retry_count = url.RetryCount,
            };
    }

    extension(CataloguePageUrlQuery query)
    {
        private (DynamicParameters parameters, string filterSql) WhereClause()
        {
            List<string> filterSql = [];
            DynamicParameters parameters = new();

            if (query.ProcessedOnly)
                filterSql.Add("was_processed is TRUE");
            if (query.UnprocessedOnly)
                filterSql.Add("was_processed is FALSE");
            if (query.RetryLimit.HasValue)
            {
                filterSql.Add("retry_count <= @retry");
                parameters.Add("@retry", query.RetryLimit.Value, DbType.Int32);
            }

            return filterSql.Count == 0
                ? (parameters, "")
                : (parameters, $"WHERE {string.Join(" AND ", filterSql)}");
        }

        private string LockClause()
        {
            return query.WithLock ? "FOR UPDATE" : string.Empty;
        }

        private string LimitClause()
        {
            return query.Limit.HasValue ? $"LIMIT {query.Limit.Value}" : string.Empty;
        }
    }
}
