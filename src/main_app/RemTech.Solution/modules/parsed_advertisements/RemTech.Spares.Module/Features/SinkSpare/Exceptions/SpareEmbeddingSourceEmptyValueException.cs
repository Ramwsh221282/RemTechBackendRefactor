using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Exceptions;

internal sealed class SpareEmbeddingSourceEmptyValueException : Exception
{
    public SpareEmbeddingSourceEmptyValueException(string fieldName)
        : base($"Поле {fieldName} было пустым для {nameof(SpareEmbeddingStringSource)}") { }

    public SpareEmbeddingSourceEmptyValueException(string fieldName, Exception inner)
        : base($"Поле {fieldName} было пустым для {nameof(SpareEmbeddingStringSource)}", inner) { }
}
