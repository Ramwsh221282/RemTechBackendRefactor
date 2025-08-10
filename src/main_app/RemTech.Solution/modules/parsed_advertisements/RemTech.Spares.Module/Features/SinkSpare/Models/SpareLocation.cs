using Npgsql;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Json;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class SpareLocation(Guid id, string location, string kind)
    : ISpareEmbeddingSourceModification,
        ISpareJsonObjectModifier,
        ISparePersistanceCommandSource<NpgsqlCommand, SpareSqlPersistanceCommand>
{
    public void Modify(SpareEmbeddingStringSource source)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new SpareEmbeddingSourceEmptyValueException(nameof(location));
        if (string.IsNullOrWhiteSpace(kind))
            throw new SpareEmbeddingSourceEmptyValueException(nameof(kind));
        string text = $"{location} {kind}";
        source.Add(text);
    }

    public void Modify(SpareJsonObject jsonObject)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(location));
        if (string.IsNullOrWhiteSpace(kind))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(kind));
        if (id == Guid.Empty)
            throw new SpareJsonObjectModifierValueEmptyException(nameof(id));
        jsonObject.Add("location", location);
        jsonObject.Add("location_id", id);
        jsonObject.Add("location_kind", kind);
    }

    public SpareSqlPersistanceCommand Modify(SpareSqlPersistanceCommand persistance)
    {
        if (id == Guid.Empty)
            throw new SparePersistanceCommandEmptyValueException(nameof(id));
        NpgsqlCommand command = persistance.Read();
        command.Parameters.Add(new NpgsqlParameter<Guid>("@geo_id", id));
        return new SpareSqlPersistanceCommand(command);
    }
}
