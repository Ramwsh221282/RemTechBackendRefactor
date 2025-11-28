using System.Data;
using Dapper;
using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserWorkTurning;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.ParserStateManagement.EventListeners;

public sealed class NpgSqlOnStatefulParserStateChangedEventListener(
 NpgSqlSession session
) : IOnStatefulParserStateChangedEventListener
{
 public async Task<Result<Unit>> React(RegisteredParser parser, ParserWorkTurner turner, CancellationToken ct = default)
 {
  const string sql = """
                     UPDATE parsers_control_module.work_states
                     SET state = @state
                     WHERE id = @id
                     """;
  NpgSqlParserStateUpdateParameters parameters = new(parser, turner);
  if (!parameters.AreAboutSameParser()) return Error.Conflict("Несоответствие идентификаторов парсеров.");
  CommandDefinition command = session.FormCommand(sql, parameters.Read(), ct);
  await session.Execute(command);
  return Unit.Value;
 }

 private sealed class NpgSqlParserStateUpdateParameters
 {
  private readonly DynamicParameters _parameters = new();
  private Guid _turnerId = Guid.Empty;
  private Guid _parserId = Guid.Empty;
  private void WriteId(Guid id) => _parameters.Add("@id", id, DbType.Guid);
  private void WriteState(string state) => _parameters.Add("@state", state, DbType.String);
  private void WriteTurnerIdForComparison(Guid id) => _turnerId = id;
  private void WriteParserId(Guid id) => _parserId = id;

  public bool AreAboutSameParser()
  {
   return _turnerId == _parserId;
  }

  public DynamicParameters Read()
  {
   return _parameters;
  }
  
  public NpgSqlParserStateUpdateParameters(RegisteredParser parser, ParserWorkTurner turner)
  {
   parser.Write(WriteParserId);
   turner.Write(WriteId, WriteState);
   turner.Write(WriteTurnerIdForComparison);
  }
 }
}