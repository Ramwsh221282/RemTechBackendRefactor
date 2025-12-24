using System.Data;
using Dapper;
using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.NpgSql;

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
        string lockClause = LockClause(query);
        string sql = $"""
                      SELECT id as id, 
                             type as type, 
                             domain as domain, 
                             processed as processed, 
                             elapsed_seconds as elapsed_seconds, 
                             state as state,
                             started_at as started_at,
                             finished_at as finished_at,
                             wait_days as wait_days,
                             next_run as next_run
                      FROM parsers_control_module.registered_parsers
                      {filterSql}
                      {lockClause}
                      LIMIT 1
                      """;
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        SubscribedParser? result = await session.QuerySingleUsingReader(command, MapFromReader);
        if (result == null) return Error.NotFound("Парсер не найден.");
        return new SqlSpeakingParser(session, result, ct);
    }

    private static SubscribedParser MapFromReader(IDataReader reader)
    {
        Guid id = reader.GetValue<Guid>("id");
        string type = reader.GetValue<string>("type");
        string domain = reader.GetValue<string>("domain");
        int processed = reader.GetValue<int>("processed");
        long elapsedSeconds = reader.GetValue<long>("elapsed_seconds");
        string state = reader.GetValue<string>("state");
        DateTime? startedAt = reader.GetNullable<DateTime>("started_at");
        DateTime? finishedAt = reader.GetNullable<DateTime>("finished_at");
        DateTime? nextRun = reader.GetNullable<DateTime>("next_run");
        int? waitDays = reader.GetNullable<int>("wait_days");

        SubscribedParserId parserId = SubscribedParserId.Create(id);
        SubscribedParserIdentity identity = SubscribedParserIdentity.Create(type, domain);
        SubscribedParserStatistics statistics = new(
            SubscribedParserWorkTimeStatistics.FromTotalElapsedSeconds(elapsedSeconds), 
            SubscribedParserParsedCount.Create(processed));
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
            filterSql.Add("id=@id");
            parameters.Add("@id", query.Id.Value, DbType.Guid);
        }

        if (!string.IsNullOrWhiteSpace(query.Domain))
        {
            filterSql.Add("domain=@domain");
            parameters.Add("@domain", query.Domain, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            filterSql.Add("type=@type");
            parameters.Add("@type", query.Type, DbType.String);
        }
        
        return filterSql.Count == 0 ? (parameters, "") : (parameters, $" WHERE {string.Join(" AND ", filterSql)}");
    }

    private static string LockClause(SubscribedParserQuery query)
    {
        return query.WithLock ? "FOR UPDATE" : "";
    }
}