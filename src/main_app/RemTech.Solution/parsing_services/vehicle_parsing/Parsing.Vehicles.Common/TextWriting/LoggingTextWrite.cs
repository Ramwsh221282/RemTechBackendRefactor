using Serilog;

namespace Parsing.Vehicles.Common.TextWriting;

public sealed class LoggingTextWrite : ITextWrite
{
    private readonly ITextWrite _write;
    private readonly ILogger _logger;

    public LoggingTextWrite(ILogger logger, ITextWrite write)
    {
        _logger = logger;
        _write = write;
    }
    
    public void Dispose()
    {
        _logger.Information("Text writer dispoed.");;
    }

    public async ValueTask DisposeAsync()
    {
        await _write.DisposeAsync();
        _logger.Information("Text writer dispoed.");
    }

    public async Task<bool> WriteAsync(string text)
    {
        bool written = await _write.WriteAsync(text);
        _logger.Information("Text {0}. Written: {1}.", text, written);
        return written;
    }
}