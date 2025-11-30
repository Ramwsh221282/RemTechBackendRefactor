using System.Data;
using Dapper;
using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.NpgSql;

public sealed class NpgSqlParserSchedulesStorage(NpgSqlSession session) : IParserScheduleStorage
{
    public async Task<ParserSchedule?> Fetch(
        ParserScheduleQueryArgs args, 
        CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(args);
        string lockClause = LockClause(args);
        string sql = $"""
                     SELECT
                     id,
                     finished_at,
                     wait_days
                     FROM parsers_control_module.schedules
                     {filterSql}
                     {lockClause}
                     LIMIT 1
                     """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlParserSchedule? schedule = await session.QueryMaybeRow<NpgSqlParserSchedule?>(command);
        return schedule?.ToParserSchedule();
    }

    public async Task Persist(ParserSchedule instance, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO parsers_control_module.schedules
                           (id, finished_at, wait_days)
                           VALUES
                           (@id, @finished_at, @wait_days)
                           """;
        NpgSqlParserScheduleParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    public async Task Update(ParserSchedule instance, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE parsers_control_module.schedules
                           SET
                               finished_at = @finished_at,
                               wait_days = @wait_days
                           WHERE
                               id = @id
                           """;
        NpgSqlParserScheduleParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }

    private static (DynamicParameters parameters, string filterSql) WhereClause(ParserScheduleQueryArgs args)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (args.Id.HasValue)
        {
            filters.Add("id=@id");
            parameters.Add("@id", args.Id.Value, DbType.Guid);
        }

        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return  (parameters, sql);
    }

    private static string LockClause(ParserScheduleQueryArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }

    private sealed class NpgSqlParserSchedule
    {
        public required Guid Id { get; init; }
        public required DateTime? FinishedAt { get; init; }
        public required int? WaitDays { get; init; }

        public ParserSchedule ToParserSchedule()
        {
            ParserScheduleData data = new(Id, FinishedAt, WaitDays);
            return new ParserSchedule(data);
        }
    }
    
    private sealed class NpgSqlParserScheduleParameters
    {
        private readonly DynamicParameters _parameters = new();
        private void AddId(Guid id) => _parameters.AddParameter(id, "@id");
        private void AddDays(int? days) => _parameters.AddParameter(days, "@wait_days");
        private void AddEnd(DateTime? end) => _parameters.AddParameter(end, "@finished_at");
        public DynamicParameters Read() => _parameters;
        public NpgSqlParserScheduleParameters(ParserSchedule schedule) =>
            schedule.Write(
                writeId: AddId, 
                writeEnd: AddEnd, 
                writeDays: AddDays
            );
    }
}