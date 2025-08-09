using Npgsql;
using NpgsqlTypes;
using Pgvector;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Database.Embeddings;
using RemTech.Vehicles.Module.Types.Transport.Decorators.Json;
using RemTech.Vehicles.Module.Utilities;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators.Postgres;

public sealed class PgSavingVehicle(Vehicle vehicle, IEmbeddingGenerator generator)
    : Vehicle(vehicle)
{
    public async Task<int> SaveAsync(NpgsqlConnection connection, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.parsed_vehicles
            (id, kind_id, brand_id, geo_id, model_id, price, is_nds, source_url, source_domain, object, description, document_tsvector, embedding)
            VALUES
            (@id, @kind_id, @brand_id, @geo_id, @model_id, @price, @is_nds, @source_url, @source_domain, @object, @description, to_tsvector('russian', @document_tsvector), @embedding)
            ON CONFLICT(id) DO NOTHING;
            """
        );
        int affected = await new AsyncExecutedCommand(
            new AsyncPreparedCommand(MakeParametrizedCommand(connection, sql))
        ).AsyncExecuted(ct);
        return affected != 1
            ? throw new OperationException(
                $"Не удается добавить объявление. Объявление с ID: {(string)Identity.Read()} уже присутствует"
            )
            : affected;
    }

    private ParametrizingPgCommand MakeParametrizedCommand(NpgsqlConnection connection, string sql)
    {
        string id = Identity.Read();
        Guid kindId = Kind.Id();
        Guid brandId = Brand.Id();
        Guid geoId = Location.Id();
        Guid modelId = Model.Id();
        long price = Price.Value();
        bool nds = Price.UnderNds();
        string sourceUrl = SourceUrl;
        string sourceDomain = SourceDomain;
        return new ParametrizingPgCommand(new PgCommand(connection, sql))
            .With("@embedding", new Vector(MakeVectorEmbedding(out string description)))
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
            .With("@object", new JsonVehicle(this).Read(), NpgsqlDbType.Jsonb)
            .With("@document_tsvector", MakeTsVectorString(), NpgsqlDbType.Text);
    }

    private float[] MakeVectorEmbedding(out string description)
    {
        description = string.Empty;
        string kindName = Kind.Name();
        string brandName = Brand.Name();
        string modelName = Model.NameString();
        string geoName = Location.Name();
        IEnumerable<string> ctxes = Characteristics.Read().Select(c => c.NameValued());
        string[] texts = [kindName, brandName, modelName, geoName];
        texts = [.. texts, .. ctxes, new StringForVectorStoring(Description).Read()];
        string finalText = string.Join(" ", texts);
        description = finalText;
        float[] embeddings = generator.Generate(finalText);
        return embeddings;
    }

    private string MakeTsVectorString()
    {
        string kindName = Kind.Name();
        string brandName = Brand.Name();
        string modelName = Model.NameString();
        IEnumerable<string> ctxes = Characteristics.Read().Select(c => c.NameValued());
        string[] texts = [kindName, brandName, modelName];
        texts = [.. texts, .. ctxes];
        string tsVectorText = string.Join(" ", texts);
        return tsVectorText;
    }
}
