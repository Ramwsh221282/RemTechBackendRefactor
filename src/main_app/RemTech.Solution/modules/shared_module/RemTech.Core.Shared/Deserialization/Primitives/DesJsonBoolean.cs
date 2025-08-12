using System.Text.Json;

namespace RemTech.Core.Shared.Deserialization.Primitives;

public sealed class DesJsonBoolean
{
    private readonly bool _value;

    public DesJsonBoolean(DesJsonElement element)
        : this((JsonElement)element) { }

    public DesJsonBoolean(JsonElement element) => _value = element.GetBoolean();

    public static implicit operator bool(DesJsonBoolean value) => value._value;
}
