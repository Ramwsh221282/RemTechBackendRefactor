using System.Text.Json;

namespace RemTech.Json.Library.Deserialization.Primitives;

public sealed class DesJsonDateTime
{
    private readonly DateTime _value;

    public DesJsonDateTime(DesJsonElement element)
        : this((JsonElement)element) { }

    public DesJsonDateTime(JsonElement element) => _value = element.GetDateTime();

    public static implicit operator DateTime(DesJsonDateTime value) => value._value;
}
