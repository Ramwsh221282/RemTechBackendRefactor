using System.Data;
using Dapper;
using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserWorkStateManagement.NpgSql;

public sealed class NpgSqlParserWorkStatesStorage(NpgSqlSession session) : IParserWorkStatesStorage
{
    public async Task Persist(ParserWorkTurner instance, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO parsers_control_module.work_states
                           (id, state)
                           VALUES
                           (@id, @state)
                           """;
        NpgSqlParserWorkTurnerParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }
    
    public async Task<ParserWorkTurner?> Fetch(ParserWorkTurnerQueryArgs args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = WhereClause(args);
        string lockClause = LockClause(args);
        string sql = $"""
                      SELECT
                      id,
                      state
                      FROM  parsers_control_module.work_states
                      {filterSql}
                      {lockClause}
                      LIMIT 1
                      """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlParserWorkTurner? turner = await session.QueryMaybeRow<NpgSqlParserWorkTurner?>(command);
        return turner?.ToWorkTurner();
    }

    public async Task Update(ParserWorkTurner instance, CancellationToken ct = default)
    {
        const string sql = """
                           UPDATE  parsers_control_module.work_states
                           SET state = @state
                           WHERE id = @id;
                           """;
        NpgSqlParserWorkTurnerParameters parameters = new(instance);
        CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
        await session.Execute(command);
    }
    
    private static (DynamicParameters parameters, string filterSql) WhereClause(ParserWorkTurnerQueryArgs args)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (args.Id.HasValue)
        {
            filters.Add("id=@id");
            parameters.AddParameter(args.Id, "@id");
        }

        string sql = filters.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filters);
        return (parameters, sql);
    }

    private static string LockClause(ParserWorkTurnerQueryArgs args)
    {
        return args.WithLock ? "FOR UPDATE" : string.Empty;
    }

    private sealed class NpgSqlParserWorkTurner
    {
        public required Guid Id { get; init; }
        public required string State { get; init; }

        public ParserWorkTurner ToWorkTurner()
        {
            Result<ParserState> state = ParserState.FromString(State);
            if (state.IsFailure) 
                throw new InvalidOperationException($"Unable to convert: {nameof(NpgSqlParserWorkTurner)} to {nameof(ParserState)}. State is not supported");
            ParserWorkTurnerState turnerState = new(Id, state);
            ParserWorkTurner turner = new(turnerState);
            return turner;
        }
    }
    
    private sealed class NpgSqlParserWorkTurnerParameters
    {
        private readonly DynamicParameters _parameters = new();
        private void AddId(Guid id) => _parameters.Add("@id", id, DbType.Guid);
        private void AddState(string state) => _parameters.Add("@state", state, DbType.String);
        public DynamicParameters Read() => _parameters;
        public NpgSqlParserWorkTurnerParameters(ParserWorkTurner turner) => turner.Write(AddId, AddState);
    }
}