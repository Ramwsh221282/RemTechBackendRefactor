using Npgsql;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Json;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class Spare(
    string id,
    SpareTextInformation text,
    SpareLocation location,
    SparePrice price,
    SpareSourceInformation sourceInfo,
    SparePhotos photos
)
    : ISpareEmbeddingSourceModification,
        ISparePersistanceCommandSource<NpgsqlCommand, SpareSqlPersistanceCommand>,
        ISpareJsonObjectModifier
{
    public void Modify(SpareEmbeddingStringSource source)
    {
        text.Modify(source);
        location.Modify(source);
        price.Modify(source);
    }

    public SpareSqlPersistanceCommand Modify(SpareSqlPersistanceCommand persistance)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new SparePersistanceCommandEmptyValueException(nameof(id));
        NpgsqlCommand command = persistance.Read();
        command.Parameters.Add(new NpgsqlParameter<string>("@id", id));
        SpareSqlPersistanceCommand withId = new(command);
        SpareSqlPersistanceCommand withPrice = price.Modify(withId);
        SpareSqlPersistanceCommand withLocation = location.Modify(withPrice);
        SpareSqlPersistanceCommand withSource = sourceInfo.Modify(withLocation);
        return withSource;
    }

    public void Modify(SpareJsonObject jsonObject)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(id));
        jsonObject.Add("id", id);
        text.Modify(jsonObject);
        price.Modify(jsonObject);
        location.Modify(jsonObject);
        photos.Modify(jsonObject);
        sourceInfo.Modify(jsonObject);
    }
}
