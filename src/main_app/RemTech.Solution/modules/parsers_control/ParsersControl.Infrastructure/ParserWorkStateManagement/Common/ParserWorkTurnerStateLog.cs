using ParsersControl.Core.ParserWorkStateManagement;

namespace ParsersControl.Infrastructure.ParserWorkStateManagement.Common;

public sealed class ParserWorkTurnerStateLog
{
    private readonly Serilog.ILogger _logger;
    private Guid _id = Guid.Empty;
    private string _state = string.Empty;
    private void AddId(Guid id) => _id = id;
    private void AddState(string state) => _state = state;

    public void Log()
    {
        object[] logProperties = [_id, _state];
        _logger.Information("""
                            Parser work state info:
                            Id: {Id}
                            State: {State}
                            """, logProperties);
    }
    
    public ParserWorkTurnerStateLog(Serilog.ILogger logger, ParserWorkTurnerState state)
    {
        _logger = logger;
        state.Write(AddId, AddState);
    }
}