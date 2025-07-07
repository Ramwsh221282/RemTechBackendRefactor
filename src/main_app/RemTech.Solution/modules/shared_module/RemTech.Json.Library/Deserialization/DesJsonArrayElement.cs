using System.Text.Json;

namespace RemTech.Json.Library.Deserialization;

public sealed class DesJsonArrayElement
{
    private readonly JsonElement _element;

    public DesJsonArrayElement(JsonElement element)
    {
        _element = element;
    }

    public JsonElement Property(string key)
    {
        if (!_element.TryGetProperty(key, out JsonElement prop))
            throw new KeyNotFoundException(
                $"Элемент json массива не содержит поле с ключем: {key}"
            );
        return prop;
    }

    public JsonElement this[string key] => Property(key);
}
