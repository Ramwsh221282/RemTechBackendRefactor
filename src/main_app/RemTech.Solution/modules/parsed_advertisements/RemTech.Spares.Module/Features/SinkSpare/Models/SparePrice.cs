using Npgsql;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Json;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class SparePrice(long value, bool isNds)
    : ISpareEmbeddingSourceModification,
        ISparePersistanceCommandSource<NpgsqlCommand, SpareSqlPersistanceCommand>,
        ISpareJsonObjectModifier
{
    public void Modify(SpareEmbeddingStringSource source)
    {
        string text = $"Цена: {value}. {(isNds ? "с НДС" : "без НДС")}";
        source.Add(text);
    }

    public SpareSqlPersistanceCommand Modify(SpareSqlPersistanceCommand persistance)
    {
        if (value < 0)
            throw new SparePersistanceCommandEmptyValueException(nameof(value));
        NpgsqlCommand command = persistance.Read();
        command.Parameters.Add(new NpgsqlParameter<long>("@price", value));
        command.Parameters.Add(new NpgsqlParameter<bool>("@is_nds", isNds));
        return new SpareSqlPersistanceCommand(command);
    }

    public void Modify(SpareJsonObject jsonObject)
    {
        if (value < 0)
            throw new SpareJsonObjectModifierValueEmptyException(nameof(value));
        jsonObject.Add("priceValue", value);
        jsonObject.Add("isNds", isNds);
    }
}
