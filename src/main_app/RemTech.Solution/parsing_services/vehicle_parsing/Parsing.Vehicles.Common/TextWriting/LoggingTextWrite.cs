using RemTech.Logging.Library;

namespace Parsing.Vehicles.Common.TextWriting;

public sealed class LoggingTextWrite : ITextWrite
{
    private readonly ITextWrite _write;

    public LoggingTextWrite(ITextWrite write)
    {
        _write = write;
    }
    
    public void Dispose()
    {
        Console.WriteLine("Text writer dispoed.");;
    }

    public async ValueTask DisposeAsync()
    {
        await _write.DisposeAsync();
        Console.WriteLine("Text writer dispoed.");
    }

    public async Task<bool> WriteAsync(string text)
    {
        bool written = await _write.WriteAsync(text);
        Console.WriteLine("Text {0}. Written: {1}.", text, written);
        return written;
    }
}