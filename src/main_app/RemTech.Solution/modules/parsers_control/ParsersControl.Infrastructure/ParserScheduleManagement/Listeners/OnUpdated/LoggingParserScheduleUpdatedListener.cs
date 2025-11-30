using ParsersControl.Core.ParserScheduleManagement;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using ParsersControl.Infrastructure.ParserScheduleManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnUpdated;

public sealed class LoggingParserScheduleUpdatedListener(Serilog.ILogger logger) : IParserScheduleUpdatedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IParserScheduleUpdatedEventListener>();
    
    public Task<Result<Unit>> React(ParserScheduleData data, CancellationToken ct = default)
    {
        ParserScheduleLog log = new(_logger, data);
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}