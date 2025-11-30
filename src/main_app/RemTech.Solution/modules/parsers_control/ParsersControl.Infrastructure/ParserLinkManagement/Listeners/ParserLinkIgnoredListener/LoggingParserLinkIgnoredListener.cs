using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkIgnoredListener;

public sealed class LoggingParserLinkIgnoredListener(Serilog.ILogger logger) : IParserLinkIgnoredListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IParserLinkIgnoredListener>();
    
    public Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        ParserLinkDataLog log = new(new ParserLink(data), _logger);
        _logger.Information("Parser link ignored.");
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}