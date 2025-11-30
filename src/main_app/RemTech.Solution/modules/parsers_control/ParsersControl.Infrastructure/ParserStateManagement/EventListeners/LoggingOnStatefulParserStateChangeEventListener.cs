using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserStateManagement.EventListeners;

public sealed class LoggingOnStatefulParserStateChangeEventListener(
    Serilog.ILogger logger
)
    : IOnStatefulParserStateChangedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IOnStatefulParserStateChangedEventListener>();
    private readonly Queue<IOnStatefulParserStateChangedEventListener> _listeners = [];

    public LoggingOnStatefulParserStateChangeEventListener Wrap(
        IOnStatefulParserStateChangedEventListener listener
    )
    {
        _listeners.Enqueue(listener);
        return this;
    }
 
    public async Task<Result<Unit>> React(RegisteredParser parser, ParserWorkTurner turner, CancellationToken ct = default)
    {
        _logger.Information("Updating parser work state.");
        while (_listeners.Count > 0)
        {
            IOnStatefulParserStateChangedEventListener listener = _listeners.Dequeue();
            Result<Unit> update = await listener.React(parser, turner, ct);
            if (update.IsFailure) return update.Error;
        }

        ParserStateLog log = new(parser, turner);
        log.Log(_logger);
        return Unit.Value;
    }

    private sealed class ParserStateLog
    {
        private Guid _id = Guid.Empty;
        private string _domain = string.Empty;
        private string _type = string.Empty;
        private string _state = string.Empty;
        private void AddId(Guid id) => _id = id;
        private void AddDomain(string domain) => _domain = domain;
        private void AddType(string type) => _type = type;
        private void AddState(string state) => _state = state;
        public ParserStateLog(RegisteredParser parser, ParserWorkTurner turner)
        {
            parser.Write(AddId, AddDomain, AddType);
            turner.Write(writeState: AddState);
        }

        public void Log(Serilog.ILogger logger)
        {
            object[] logProperties = [_id, _domain, _type, _state];
            logger.Information("""
                               Parser state info:
                               ID: {id}
                               Domain: {domain}
                               Type: {type}
                               State {state}
                               """, logProperties);
        }
    }
}