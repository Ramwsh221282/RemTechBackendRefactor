using Npgsql;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Json;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class SpareLocation(
    Guid regionId,
    Guid cityId,
    string location,
    string kind,
    string city
)
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
        string text = $"{location} {kind} {city}";
        source.Add(text);
    }

    public void Modify(SpareJsonObject jsonObject)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(location));
        if (string.IsNullOrWhiteSpace(kind))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(kind));
        if (string.IsNullOrWhiteSpace(city))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(city));
        if (regionId == Guid.Empty)
            throw new SpareJsonObjectModifierValueEmptyException(nameof(regionId));
        if (cityId == Guid.Empty)
            throw new SpareJsonObjectModifierValueEmptyException(nameof(cityId));
        jsonObject.Add("region", location);
        jsonObject.Add("region_id", regionId);
        jsonObject.Add("region_kind", kind);
        jsonObject.Add("city", city);
        jsonObject.Add("city_id", cityId);
    }

    public SpareSqlPersistanceCommand Modify(SpareSqlPersistanceCommand persistance)
    {
        if (regionId == Guid.Empty)
            throw new SparePersistanceCommandEmptyValueException(nameof(regionId));
        NpgsqlCommand command = persistance.Read();
        command.Parameters.Add(new NpgsqlParameter<Guid>("@region_id", regionId));
        command.Parameters.Add(new NpgsqlParameter<Guid>("@city_id", regionId));
        return new SpareSqlPersistanceCommand(command);
    }
}
