using System.Text;
using System.Text.Json;

namespace Tickets.EventListeners.Points.TicketCreated;

public sealed class JsonMessageFromRabbitMq(ReadOnlyMemory<byte> body) : IDisposable
{
    private readonly JsonDocument _document = JsonDocument.Parse(Encoding.UTF8.GetString(body.Span));
    private JsonElement _root => _document.RootElement;

    public bool TryGetProperty(ReadOnlySpan<char> propertyName, out JsonElement element)
    {
        return _root.TryGetProperty(propertyName, out element);
    }

    public void Dispose()
    {
        _document.Dispose();
    }
}