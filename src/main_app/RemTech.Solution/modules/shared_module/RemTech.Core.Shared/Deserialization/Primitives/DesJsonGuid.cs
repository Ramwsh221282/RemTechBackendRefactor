using System.Text.Json;

namespace RemTech.Core.Shared.Deserialization.Primitives;

public sealed class DesJsonGuid
{
    private readonly Guid _value;

    public DesJsonGuid(DesJsonElement element)
        : this((JsonElement)element) { }

    public DesJsonGuid(JsonElement element) => _value = element.GetGuid();

    public static implicit operator Guid(DesJsonGuid value) => value._value;
}
