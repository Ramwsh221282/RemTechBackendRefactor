using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Json;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class SpareTextInformation(string description, string title)
    : ISpareEmbeddingSourceModification,
        ISpareJsonObjectModifier
{
    public void Modify(SpareEmbeddingStringSource source)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new SpareEmbeddingSourceEmptyValueException(nameof(description));
        if (string.IsNullOrWhiteSpace(title))
            throw new SpareEmbeddingSourceEmptyValueException(nameof(title));
        source.Add(title);
        source.Add(description);
    }

    public void Modify(SpareJsonObject jsonObject)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(description));
        if (string.IsNullOrWhiteSpace(title))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(title));
        jsonObject.Add("description", description);
        jsonObject.Add("title", title);
    }
}
