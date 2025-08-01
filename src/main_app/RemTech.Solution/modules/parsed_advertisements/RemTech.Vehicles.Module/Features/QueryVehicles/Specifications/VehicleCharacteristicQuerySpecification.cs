using Npgsql;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

public sealed class VehicleCharacteristicQuerySpecification(
    IEnumerable<VehicleCharacteristic> characteristic
) : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        int number = 1;
        foreach (VehicleCharacteristic ctx in characteristic)
        {
            Guid id = ctx.WhatCharacteristic().Identity.ReadId();
            string value = ctx.WhatValue();
            string existsSubQuery = $"""
                EXISTS 
                ( 
                SELECT 1 FROM parsed_advertisements_module.parsed_vehicle_characteristics pvc{number}
                WHERE pvc{number}.vehicle_id = v.id
                AND pvc{number}.ctx_id = @ctxId_{number}
                AND pvc{number}.ctx_value = @ctxValue_{number}   
                )
                """;
            NpgsqlParameter<Guid> idParam = new NpgsqlParameter<Guid>($"@ctxId_{number}", id);
            NpgsqlParameter<string> valueParam = new NpgsqlParameter<string>(
                $"@ctxValue_{number}",
                value
            );
            query.AcceptFilter(existsSubQuery, [idParam, valueParam]);
            number++;
        }
    }
}
