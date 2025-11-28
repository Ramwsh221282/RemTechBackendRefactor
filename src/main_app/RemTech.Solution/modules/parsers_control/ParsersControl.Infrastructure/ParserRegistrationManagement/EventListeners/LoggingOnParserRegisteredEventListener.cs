using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;

public sealed class LoggingOnParserRegisteredEventListener(
    Serilog.ILogger logger
    )
    : IOnParserRegisteredEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IOnParserRegisteredEventListener>();
    private readonly Queue<IOnParserRegisteredEventListener> _listeners = [];
    
    public async Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
    {
        _logger.Information("Registering parser.");
        while (_listeners.Count > 0)
        {
            IOnParserRegisteredEventListener listener =  _listeners.Dequeue();
            Result<Unit> result = await listener.React(data, ct);
            if (result.IsFailure)
            {
                _logger.Error("Registering parser failed. ERROR: {Error}", result.Error.Message);
                return result.Error;
            }
        }
        
        object[] logProperties = [data.Id, data.Domain, data.Type];
        _logger.Information("Registered parser: {Id} {Domain} {Type}", logProperties);
        return Unit.Value;
    }

    public LoggingOnParserRegisteredEventListener Wrap(IOnParserRegisteredEventListener listener)
    {
        _listeners.Enqueue(listener);
        return this;
    }
}