using System.Data;
using Dapper;
using ParsersControl.Core.Common;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ParsersControl.Infrastructure.Parsers.Repository;

public sealed class SubscribedParsersRepository(NpgSqlSession session) : ISubscribedParsersRepository
{
    public Task<bool> Exists(SubscribedParserIdentity identity, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT EXISTS 
                               (SELECT 1 FROM parsers_control_module.registered_parsers
                                         WHERE domain = @domain AND type = @type)
                           """;
        object parameters = new { domain = identity.DomainName, type = identity.ServiceType };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        return session.QuerySingleRow<bool>(command);
    }

    public Task Add(SubscribedParser parser, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO parsers_control_module.registered_parsers
                               (id, type, domain, state, processed, elapsed_seconds, started_at, finished_at, wait_days, next_run)
                           VALUES
                               (@id, @type, @domain, @state, @processed, @elapsed_seconds, @started_at, @finished_at, @wait_days, @next_run)
                           """;
        object parameters = new
        {
            id = parser.Id.Value,
            type = parser.Identity.ServiceType,
            domain = parser.Identity.DomainName,
            state = parser.State.Value,
            processed = parser.Statistics.ParsedCount.Value,
            elapsed_seconds = parser.Statistics.WorkTime.TotalElapsedSeconds,
            started_at = parser.Schedule.StartedAt == null ? (object?)null : parser.Schedule.StartedAt.Value,
            finished_at = parser.Schedule.FinishedAt == null ? (object?)null : parser.Schedule.FinishedAt.Value,
            wait_days = parser.Schedule.WaitDays == null ? (object?)null : parser.Schedule.WaitDays.Value,
            next_run = parser.Schedule.NextRun == null ? (object?)null : parser.Schedule.NextRun.Value
        };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        return session.Execute(command);
    }

    public async Task Save(ISubscribedParser parser)
    {
        if (parser is SqlSpeakingParser speaking)
        {
            await speaking.Save();
            return;
        }
        throw new NotSupportedException($"Unsupported parser type for saving: {parser.GetType().Name}");
    }

    public async Task<Result<ISubscribedParser>> Get(SubscribedParserQuery query, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(query);
         if (query.WithLock) await Block(query, ct);
         string sql = $"""
                       SELECT p.id as parser_id, 
                              p.type as parser_type, 
                              p.domain as parser_domain, 
                              p.processed as parser_processed, 
                              p.elapsed_seconds as parser_elapsed_seconds, 
                              p.state as parser_state,
                              p.started_at as parser_started_at,
                              p.finished_at as parser_finished_at,
                              p.wait_days as parser_wait_days,
                              p.next_run as parser_next_run,
                              l.id as link_id,
                              l.parser_id as link_parser_id,
                              l.name as link_name,
                              l.url as link_url,
                              l.is_active as link_is_active,
                              l.processed as link_processed,
                              l.elapsed_seconds as link_elapsed_seconds
                       FROM parsers_control_module.registered_parsers p
                       LEFT JOIN parsers_control_module.parser_links l
                           ON p.id = l.parser_id
                       {filterSql}
                       """;

        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        
        (SubscribedParser? parser, List<SubscribedParserLink> links) = 
            await session.QuerySingleUsingReader(command, MapFromReader, MapLinkFromReader, new ParserIdComparer());

        return parser == null
            ? Error.NotFound("Парсер не найден.")
            : new SqlSpeakingParser(session, new SubscribedParser(parser, links), ct);
    }

    private sealed class ParserIdComparer : IEqualityComparer<SubscribedParser>
    {
        public bool Equals(SubscribedParser? x, SubscribedParser? y)
        {
            if (x is null || y is null) return x == y;
            return x.Id == y.Id;
        }

        public int GetHashCode(SubscribedParser obj)
        {
            return obj.Id.GetHashCode();
        }
    }
    
    private static SubscribedParserLink? MapLinkFromReader(IDataReader reader)
    {
        if (reader.IsNull("link_id"))
            return null;
        
        Guid id = reader.GetValue<Guid>("link_id");
        Guid parserId = reader.GetValue<Guid>("link_parser_id");
        string name = reader.GetValue<string>("link_name");
        string url = reader.GetValue<string>("link_url");
        int processed = reader.GetValue<int>("link_processed");
        long elapsedSeconds = reader.GetValue<long>("link_elapsed_seconds");
        bool active = reader.GetValue<bool>("link_is_active");
        
        SubscribedParserLinkId linkId = SubscribedParserLinkId.From(id);
        SubscribedParserId parserIdVo = SubscribedParserId.Create(parserId);
        SubscribedParserLinkUrlInfo urlInfo = SubscribedParserLinkUrlInfo.Create(url, name);
        ParsingWorkTime workTime = ParsingWorkTime.FromTotalElapsedSeconds(elapsedSeconds);
        ParsedCount parsedCount = ParsedCount.Create(processed);
        ParsingStatistics statistics = new ParsingStatistics(workTime, parsedCount);
        return SubscribedParserLink.Create(parserIdVo, linkId, urlInfo, statistics, active);
    }
    
    private static SubscribedParser MapFromReader(IDataReader reader)
    {
        Guid id = reader.GetValue<Guid>("parser_id");
        string type = reader.GetValue<string>("parser_type");
        string domain = reader.GetValue<string>("parser_domain");
        int processed = reader.GetValue<int>("parser_processed");
        long elapsedSeconds = reader.GetValue<long>("parser_elapsed_seconds");
        string state = reader.GetValue<string>("parser_state");
        DateTime? startedAt = reader.GetNullable<DateTime>("parser_started_at");
        DateTime? finishedAt = reader.GetNullable<DateTime>("parser_finished_at");
        DateTime? nextRun = reader.GetNullable<DateTime>("parser_next_run");
        int? waitDays = reader.GetNullable<int>("parser_wait_days");

        SubscribedParserId parserId = SubscribedParserId.Create(id);
        SubscribedParserIdentity identity = SubscribedParserIdentity.Create(domain, type);
        ParsingStatistics statistics = new(
            ParsingWorkTime.FromTotalElapsedSeconds(elapsedSeconds), 
            ParsedCount.Create(processed));
        SubscribedParserState parserState = SubscribedParserState.FromString(state);
        SubscribedParserSchedule schedule = SubscribedParserSchedule.Create(startedAt, finishedAt, nextRun, waitDays);
        
        return new SubscribedParser(
            parserId,
            identity,
            statistics,
            parserState,
            schedule
        );
    }
    
    private static (DynamicParameters parameters, string filterSql) WhereClause(SubscribedParserQuery query)
    {
        List<string> filterSql = [];
        DynamicParameters parameters = new();
        if (query.Id.HasValue)
        {
            filterSql.Add("p.id=@id");
            parameters.Add("@id", query.Id.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.Domain))
        {
            filterSql.Add("p.domain=@domain");
            parameters.Add("@domain", query.Domain, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            filterSql.Add("p.type=@type");
            parameters.Add("@type", query.Type, DbType.String);
        }
        
        return filterSql.Count == 0 ? (parameters, "") : (parameters, $" WHERE {string.Join(" AND ", filterSql)}");
    }

    private async Task Block(SubscribedParserQuery query, CancellationToken ct)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(query);
        string sql = $"""
                           SELECT p.id 
                           FROM parsers_control_module.registered_parsers p
                           {filterSql}
                           FOR UPDATE OF p
                           """;
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        await session.Execute(command);
    }
}