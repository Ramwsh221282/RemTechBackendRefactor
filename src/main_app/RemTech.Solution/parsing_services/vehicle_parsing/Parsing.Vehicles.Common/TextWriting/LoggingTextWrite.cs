using RemTech.Logging.Library;

namespace Parsing.Vehicles.Common.TextWriting;

public sealed class LoggingTextWrite : ITextWrite
{
    private readonly ITextWrite _write;
    private readonly ICustomLogger _logger;

    public LoggingTextWrite(ICustomLogger logger, ITextWrite write)
    {
        _logger = logger;
        _write = write;
    }
    
    public void Dispose()
    {
        _logger.Info("Text writer dispoed.");;
    }

    public async ValueTask DisposeAsync()
    {
        await _write.DisposeAsync();
        _logger.Info("Text writer dispoed.");
    }

    public async Task<bool> WriteAsync(string text)
    {
        bool written = await _write.WriteAsync(text);
        _logger.Info("Text {0}. Written: {1}.", text, written);
        return written;
    }
}