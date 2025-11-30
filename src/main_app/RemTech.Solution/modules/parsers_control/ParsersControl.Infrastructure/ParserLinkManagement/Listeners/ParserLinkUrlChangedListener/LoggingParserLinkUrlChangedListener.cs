using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUrlChangedListener;

public sealed class LoggingParserLinkUrlChangedListener(Serilog.ILogger logger) : IParserLinkUrlChangedListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IParserLinkUrlChangedListener>();
    
    public Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default)
    {
        _logger.Information("Parser link url changed");
        ParserLinkDataLog log = new(new ParserLink(data), _logger);
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}