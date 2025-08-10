namespace Parsing.Avito.Common.PaginationBar;

public sealed class LoggingAvitoPaginationBarElement : IAvitoPaginationBarElement
{
    private readonly Serilog.ILogger _logger;
    private readonly IAvitoPaginationBarElement _origin;

    public LoggingAvitoPaginationBarElement(
        Serilog.ILogger logger,
        IAvitoPaginationBarElement origin
    )
    {
        _logger = logger;
        _origin = origin;
    }

    public IEnumerable<string> Iterate(string originUrl)
    {
        foreach (var item in _origin.Iterate(originUrl))
        {
            _logger.Information("Navigating {url}", item);
            yield return item;
        }
    }
}
