using System.Data;
using Dapper;
using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserStatistics.NpgSql;

public sealed class NpgSqlParserStatisticsStorage(NpgSqlSession session) : IParserStatisticsStorage
{
    public async Task Update(ParserStatistic instance, CancellationToken ct = default)
    {
        const string sql =
            """
            UPDATE parsers_control_module.statistics
            SET processed = @processed,
                elapsed_seconds = @elapsed_seconds
            WHERE id = @id
            """;
        NpgSqlParserStatisticParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    public async Task Persist(ParserStatistic instance, CancellationToken ct = default)
    {
        const string sql =
            """
            INSERT INTO parsers_control_module.statistics
            (id, processed, elapsed_seconds)
            VALUES
            (@id, @processed, @elapsed_seconds);
            """;
        NpgSqlParserStatisticParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }   
    
    public async Task<ParserStatistic?> Fetch(ParserStatisticsQuery args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(args);
        string lockClause = LockClause(args);
        string sql = $"""
                      SELECT
                      id,
                      processed,
                      elapsed_seconds
                      FROM parsers_control_module.statistics
                      {filterSql}
                      {lockClause}
                      LIMIT 1
                      """;
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        NpgSqlParserStatistic? statistic = await session.QueryMaybeRow<NpgSqlParserStatistic?>(command);
        return statistic?.ToParserStatistic();
    }

    private static (DynamicParameters parameters, string filterSql) WhereClause(ParserStatisticsQuery query)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (query.Id.HasValue)
        {
            filters.Add("id=@id");
            parameters.Add("@id", query.Id.Value, DbType.Guid);
        }
        
        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, sql);
    }

    private static string LockClause(ParserStatisticsQuery query)
    {
        return query.WithLock ? "FOR UPDATE" : string.Empty;
    }

    private sealed class NpgSqlParserStatistic
    {
        public required Guid Id { get; init; }
        public required int Processed { get; init; }
        public required long ElapsedSeconds { get; init; }
        public ParserStatistic ToParserStatistic() => new(new ParserStatisticData(Id, Processed, ElapsedSeconds));
    }
    
    private sealed class NpgSqlParserStatisticParameters
    {
        private readonly DynamicParameters _parameters = new DynamicParameters();
        private void AddId(Guid id) => _parameters.Add("@id", id, DbType.Guid);
        private void AddProcessed(int processed) => _parameters.Add("@processed", processed, DbType.Int32);
        private void AddElapsedSeconds(long elapsedSeconds) =>
            _parameters.Add("@elapsed_seconds", elapsedSeconds);
        public DynamicParameters Read() => _parameters;
        public NpgSqlParserStatisticParameters(ParserStatistic statistic)
        {
            statistic.Write(
                writeId: AddId, 
                writeProcessed: AddProcessed, 
                writeElapsedSeconds: AddElapsedSeconds
                );
        }
    }
}