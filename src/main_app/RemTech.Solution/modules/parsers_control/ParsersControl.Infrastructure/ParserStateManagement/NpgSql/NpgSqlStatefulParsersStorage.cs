using Dapper;
using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserStateManagement.NpgSql;

public sealed class NpgSqlStatefulParsersStorage(NpgSqlSession session) : IStatefulParsersStorage
{
    public async Task<StatefulParser?> Fetch(StatefulParserQueryArgs args, CancellationToken ct = default)
    {
        (DynamicParameters parameters, string filterSql) = FillParameters(args);
        string lockClause = LockClause(args);
        string sql = $"""
                      SELECT
                      p.id as id,
                      p.domain as domain,
                      p.type as type,
                      s.state as state
                      FROM parsers_control_module.registered_parsers p
                      INNER JOIN parsers_control_module.work_states s ON s.id = p.id
                      {filterSql}
                      {lockClause}
                      """;
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlStatefulParserRow? row = await session.QueryMaybeRow<NpgSqlStatefulParserRow?>(command);
        return row?.ToStatefulParser();
    }

    private static string LockClause(StatefulParserQueryArgs args)
    {
        return args.WithLock ? string.Empty : "FOR UPDATE";
    }
    
    private static (DynamicParameters parameters, string filterSql) FillParameters(StatefulParserQueryArgs args)
    {
        List<string> filters = [];
        DynamicParameters parameters = new();
        if (args.Id.HasValue)
        {
            filters.Add("p.id=@id");
            parameters.Add("@id", args.Id.Value);
        }
        
        string sql = filters.Count == 0 ? string.Empty : "WHERE " +  string.Join(" AND ", filters);
        return (parameters, sql);
    }
    
    private sealed class NpgSqlStatefulParserRow
    {
        public required Guid Id { get; init; }
        public required string Domain { get; init; }
        public required string Type { get; init; }
        public required string State { get; init; }

        public StatefulParser ToStatefulParser()
        {
            Result<ParserState> state = ParserState.FromString(State);
            if (state.IsFailure) throw new InvalidOperationException($"{nameof(NpgSqlStatefulParserRow)} {state.Error.Message}");
            RegisteredParser registered = new(new ParserData(Id, Type, Domain));
            ParserWorkTurner turner = new(new ParserWorkTurnerState(Id, state));
            return new StatefulParser(registered, turner);
        }
    }
}