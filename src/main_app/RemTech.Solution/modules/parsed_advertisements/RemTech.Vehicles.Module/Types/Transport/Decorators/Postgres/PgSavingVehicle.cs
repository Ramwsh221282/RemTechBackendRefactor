using Npgsql;
using NpgsqlTypes;
using Pgvector;
using RemTech.Core.Shared.Exceptions;
using RemTech.Vehicles.Module.Types.Transport.Decorators.Json;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators.Postgres;

internal sealed class PgSavingVehicle(Vehicle vehicle, IEmbeddingGenerator generator)
    : Vehicle(vehicle)
{
    public async Task SaveAsync(NpgsqlConnection connection, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.parsed_vehicles
            (id, kind_id, brand_id, geo_id, model_id, price, is_nds, source_url, source_domain, object, description, embedding)
            VALUES
            (@id, @kind_id, @brand_id, @geo_id, @model_id, @price, @is_nds, @source_url, @source_domain, @object, @description, @embedding)
            ON CONFLICT(id) DO NOTHING;
            """
        );
        int affected = await new AsyncExecutedCommand(
            new AsyncPreparedCommand(MakeParametrizedCommand(connection, sql))
        ).AsyncExecuted(ct);
        if (affected == 0)
            throw new OperationException(
                $"Не удается добавить объявление. Объявление с ID: {(string)Identity.Read()} уже присутствует"
            );
    }

    private ParametrizingPgCommand MakeParametrizedCommand(NpgsqlConnection connection, string sql)
    {
        string id = Identity.Read();
        Guid kindId = Category.Id;
        Guid brandId = Brand.Id;
        Guid geoId = Location.Id;
        Guid modelId = Model.Id;
        long price = Price.Value();
        bool nds = Price.UnderNds();
        string sourceUrl = SourceUrl;
        string sourceDomain = SourceDomain;
        string description = MakeDocument();
        return new ParametrizingPgCommand(new PgCommand(connection, sql))
            .With("@embedding", new Vector(MakeVectorEmbedding(description)))
            .With("@id", id)
            .With("@kind_id", kindId)
            .With("@brand_id", brandId)
            .With("@geo_id", geoId)
            .With("@model_id", modelId)
            .With("@price", price)
            .With("@is_nds", nds)
            .With("@description", description)
            .With("@source_url", sourceUrl)
            .With("@source_domain", sourceDomain)
            .With("@object", new JsonVehicle(this).Read(), NpgsqlDbType.Jsonb);
    }

    private ReadOnlyMemory<float> MakeVectorEmbedding(string description)
    {
        return generator.Generate(description);
    }
}
