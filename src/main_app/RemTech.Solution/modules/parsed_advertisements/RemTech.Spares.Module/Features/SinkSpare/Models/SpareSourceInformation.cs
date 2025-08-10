using Npgsql;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Json;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Models;

internal sealed class SpareSourceInformation(string sourceUrl, string sourceDomain)
    : ISparePersistanceCommandSource<NpgsqlCommand, SpareSqlPersistanceCommand>,
        ISpareJsonObjectModifier
{
    public SpareSqlPersistanceCommand Modify(SpareSqlPersistanceCommand persistance)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
            throw new SparePersistanceCommandEmptyValueException(nameof(sourceUrl));
        if (string.IsNullOrWhiteSpace(sourceDomain))
            throw new SparePersistanceCommandEmptyValueException(nameof(sourceDomain));
        NpgsqlCommand command = persistance.Read();
        command.Parameters.Add(new NpgsqlParameter<string>("@source_url", sourceUrl));
        command.Parameters.Add(new NpgsqlParameter<string>("@source_domain", sourceDomain));
        return new SpareSqlPersistanceCommand(command);
    }

    public void Modify(SpareJsonObject jsonObject)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(sourceUrl));
        if (string.IsNullOrWhiteSpace(sourceDomain))
            throw new SpareJsonObjectModifierValueEmptyException(nameof(sourceDomain));
        jsonObject.Add("sourceUrl", sourceUrl);
        jsonObject.Add("sourceDomain", sourceDomain);
    }
}
