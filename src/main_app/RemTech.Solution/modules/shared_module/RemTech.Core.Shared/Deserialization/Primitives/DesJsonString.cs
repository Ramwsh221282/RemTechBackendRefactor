using System.Text.Json;

namespace RemTech.Core.Shared.Deserialization.Primitives;

public sealed class DesJsonString
{
    private readonly string _value;

    public DesJsonString(DesJsonElement element)
        : this((JsonElement)element) { }

    public DesJsonString(JsonElement element)
    {
        string? someString = element.GetString();
        _value = string.IsNullOrWhiteSpace(someString) ? string.Empty : someString;
    }

    public static implicit operator string(DesJsonString desJsonString) => desJsonString._value;
}
