using System.Text.Json;

namespace RemTech.Json.Library.Deserialization;

public sealed class DesJsonSource : IDisposable
{
    private readonly JsonDocument _document;

    public DesJsonSource(JsonDocument document) => _document = document;

    public DesJsonSource(string jsonString)
    {
        try
        {
            _document = JsonDocument.Parse(jsonString);
        }
        catch
        {
            throw new ArgumentException("DesJsonSource ожидает корректную json строку.");
        }
    }

    public DesJsonElement ParserElement(string key)
    {
        if (_document.RootElement.TryGetProperty(key, out JsonElement value))
            return new DesJsonElement(value);
        _document.Dispose();
        throw new ArgumentException($"Ключ: {key} не существует в DesJsonSource.");
    }

    public bool Contains(string key)
    {
        return _document.RootElement.TryGetProperty(key, out _);
    }

    public void Dispose()
    {
        _document.Dispose();
    }

    public DesJsonElement this[string key] => new(_document.RootElement.GetProperty(key));
}
