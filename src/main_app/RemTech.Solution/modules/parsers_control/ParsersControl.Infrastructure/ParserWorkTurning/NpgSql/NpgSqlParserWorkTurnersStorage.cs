using System.Data;
using Dapper;
using ParsersControl.Core.ParserWorkTurning;
using ParsersControl.Core.ParserWorkTurning.Contracts;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserWorkTurning.NpgSql;

public sealed class NpgSqlParserWorkTurnersStorage(NpgSqlSession session) : IParserWorkTurnersStorage
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

    private sealed class NpgSqlParserWorkTurnerParameters
    {
        private readonly DynamicParameters _parameters = new();
        private void AddId(Guid id) => _parameters.Add("@id", id, DbType.Guid);
        private void AddState(string state) => _parameters.Add("@state", state, DbType.String);
        public DynamicParameters Read() => _parameters;
        public NpgSqlParserWorkTurnerParameters(ParserWorkTurner turner) => turner.Write(AddId, AddState);
    }
}