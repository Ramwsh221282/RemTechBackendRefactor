using System.Text.Json;

namespace RemTech.Json.Library.Deserialization.Primitives;

public sealed class DesJsonLong
{
    private readonly long _value;

    public DesJsonLong(DesJsonElement element)
        : this((JsonElement)element) { }

    public DesJsonLong(JsonElement element) => _value = element.GetInt64();

    public static implicit operator long(DesJsonLong value) => value._value;
}
