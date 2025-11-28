using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkIgnoredListener;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUnignoredListener;

public sealed class LoggingParserLinkUnignoredListener(Serilog.ILogger logger) : IParserLinkUnignoredListener
{
    private readonly Serilog.ILogger _logger = logger;
    
    public Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        _logger.Information("Parser link unignored");
        ParserLinkDataLog log = new(new ParserLink(data), _logger);
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}