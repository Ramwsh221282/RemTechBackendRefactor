using System.Text.Json;

namespace RemTech.Json.Library.Deserialization.Primitives;

public sealed class DesJsonInteger
{
    private readonly int _value;

    public DesJsonInteger(DesJsonElement element)
        : this((JsonElement)element) { }

    public DesJsonInteger(JsonElement element) => _value = element.GetInt32();

    public static implicit operator int(DesJsonInteger integer) => integer._value;
}
