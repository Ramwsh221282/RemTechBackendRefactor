using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace Cleaner.Cleaning;

internal sealed class SuspiciousItemCollection(BasicDeliverEventArgs @event)
{
    private readonly ReadOnlyMemory<byte> _bytes = @event.Body;

    public IEnumerable<SuspiciousItem> Read()
    {
        string json = Encoding.UTF8.GetString(_bytes.Span);
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement array = document.RootElement.GetProperty("Items");
        foreach (JsonElement element in array.EnumerateArray())
        {
            string? id = element.GetProperty("Id").GetString();
            if (string.IsNullOrWhiteSpace(id))
                continue;
            string? url = element.GetProperty("SourceUrl").GetString();
            if (string.IsNullOrWhiteSpace(url))
                continue;
            string? domain = element.GetProperty("Domain").GetString();
            if (string.IsNullOrWhiteSpace(domain))
                continue;
            yield return new SuspiciousItem(id, url, domain);
        }
    }
}
