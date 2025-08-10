using RemTech.Spares.Module.Features.SinkSpare.Json;

namespace RemTech.Spares.Module.Features.SinkSpare.Exceptions;

internal sealed class SpareJsonObjectModifierValueEmptyException : Exception
{
    public SpareJsonObjectModifierValueEmptyException(string fieldName)
        : base($"Поле {fieldName} было пустым для {nameof(SpareJsonObject)}") { }

    public SpareJsonObjectModifierValueEmptyException(string fieldName, Exception inner)
        : base($"Поле {fieldName} было пустым для {nameof(SpareJsonObject)}", inner) { }
}
