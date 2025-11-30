using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using ParsersControl.Infrastructure.ParserWorkStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserWorkStateManagement.EventListeners.OnStateChanged;

public sealed class LoggingOnParserWorkStateChangedEventListener(Serilog.ILogger logger) : IParserStateChangedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IParserStateChangedEventListener>();
    
    public Task<Result<Unit>> React(ParserWorkTurnerState state, CancellationToken ct = default)
    {
        _logger.Information("Parser work state changed.");
        ParserWorkTurnerStateLog log = new(logger, state);
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}