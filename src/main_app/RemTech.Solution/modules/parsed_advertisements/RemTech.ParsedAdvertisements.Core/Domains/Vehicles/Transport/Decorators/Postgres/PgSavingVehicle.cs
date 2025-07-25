using System.Text.Json;
using Npgsql;
using NpgsqlTypes;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators.Postgres;

public sealed class PgSavingVehicle(Vehicle vehicle) : Vehicle(vehicle)
{
    public async Task<int> SaveAsync(NpgsqlConnection connection, CancellationToken ct = default)
    {
        string sql = string.Intern("""
                                   INSERT INTO parsed_advertisements_module.parsed_vehicles
                                   (id, kind_id, brand_id, geo_id, model_id, price, is_nds, photos, document_tsvector)
                                   VALUES
                                   (@id, @kind_id, @brand_id, @geo_id, @model_id, @price, @is_nds, @photos, to_tsvector('russian', @document_tsvector))
                                   ON CONFLICT(id) DO NOTHING;
                                   """);
        int affected = await
            new AsyncExecutedCommand(
                    new AsyncPreparedCommand(MakeParametrizedCommand(connection, sql)))
                .AsyncExecuted(ct);
        return affected != 1
            ? throw new OperationException(
                $"Не удается добавить объявление. Объявление с ID: {(string)Identity.Read()} уже присутствует")
            : affected;
    }

    private ParametrizingPgCommand MakeParametrizedCommand(NpgsqlConnection connection, string sql)
    {
        string id = Identity.Read();
        Guid kindId = Kind.Identify().ReadId();
        Guid brandId = Brand.Identify().ReadId();
        Guid geoId = Location.Identify().ReadId();
        Guid modelId = Model.Identity();
        long price = Price.Value();
        bool nds = Price.UnderNds();
        string photosJson = MakePhotosJsonb();
        return new ParametrizingPgCommand(new PgCommand(connection, sql))
            .With("@id", id)
            .With("@kind_id", kindId)
            .With("@brand_id", brandId)
            .With("@geo_id", geoId)
            .With("@model_id", modelId)
            .With("@price", price)
            .With("@is_nds", nds)
            .With("@photos", photosJson, NpgsqlDbType.Jsonb)
            .With("@document_tsvector", MakeTsVectorString(), NpgsqlDbType.Text);
    }

    private string MakePhotosJsonb()
    {
        string[] photos = Photos.Read().Select(p => (string)p).ToArray();
        string json = JsonSerializer.Serialize(photos);
        return json;
    }

    private string MakeTsVectorString()
    {
        string kindName = Kind.Identify().ReadText();
        string brandName = Brand.Identify().ReadText();
        string modelName = Model.Name();
        IEnumerable<string> ctxes = Characteristics.Read().Select(c => c.NameValued());
        string[] texts =
        [
            kindName,
            brandName,
            modelName,
        ];
        texts = [..texts, ..ctxes];
        string tsVectorText = string.Join(" ", texts);
        return tsVectorText;
    }
}