using System.Text;
using System.Text.Json;

namespace Tickets.EventListeners;

public sealed class JsonMessageFromRabbitMq : IDisposable
{
    private readonly JsonDocument _document;
    private readonly JsonElement _root;

    public JsonMessageFromRabbitMq(ReadOnlyMemory<byte> body)
    {
        _document = JsonDocument.Parse(Encoding.UTF8.GetString(body.Span));
        _root = _document.RootElement;
    }

    public bool TryGetProperty(ReadOnlySpan<char> propertyName, out JsonElement element)
    {
        return _root.TryGetProperty(propertyName, out element);
    }
    
    public void Dispose() => _document.Dispose();
}