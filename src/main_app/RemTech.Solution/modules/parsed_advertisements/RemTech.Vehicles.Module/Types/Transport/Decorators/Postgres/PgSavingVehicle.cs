﻿using Npgsql;
using NpgsqlTypes;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Types.Transport.Decorators.Json;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators.Postgres;

public sealed class PgSavingVehicle(Vehicle vehicle) : Vehicle(vehicle)
{
    public async Task<int> SaveAsync(NpgsqlConnection connection, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.parsed_vehicles
            (id, kind_id, brand_id, geo_id, model_id, price, is_nds, object, document_tsvector)
            VALUES
            (@id, @kind_id, @brand_id, @geo_id, @model_id, @price, @is_nds, @object, to_tsvector('russian', @document_tsvector))
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
        return new ParametrizingPgCommand(new PgCommand(connection, sql))
            .With("@id", id)
            .With("@kind_id", kindId)
            .With("@brand_id", brandId)
            .With("@geo_id", geoId)
            .With("@model_id", modelId)
            .With("@price", price)
            .With("@is_nds", nds)
            .With("@object", new JsonVehicle(this).Read(), NpgsqlDbType.Jsonb)
            .With("@document_tsvector", MakeTsVectorString(), NpgsqlDbType.Text);
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
