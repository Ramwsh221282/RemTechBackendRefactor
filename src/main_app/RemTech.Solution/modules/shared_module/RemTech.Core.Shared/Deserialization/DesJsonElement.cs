using System.Text.Json;

namespace RemTech.Core.Shared.Deserialization;

public sealed class DesJsonElement
{
    private readonly JsonElement _element;

    public DesJsonElement(JsonElement element) => _element = element;

    public static implicit operator JsonElement(DesJsonElement element) => element._element;
}
