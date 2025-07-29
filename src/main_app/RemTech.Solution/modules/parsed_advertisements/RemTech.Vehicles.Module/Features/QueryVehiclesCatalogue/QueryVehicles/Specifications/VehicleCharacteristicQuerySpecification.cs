using Npgsql;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public sealed class VehicleCharacteristicQuerySpecification(
    IEnumerable<VehicleCharacteristic> characteristic
) : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        int number = 1;
        foreach (VehicleCharacteristic characteristic1 in characteristic)
        {
            ApplyId(characteristic1, query, ref number);
            ApplyName(characteristic1, query, ref number);
            ApplyValue(characteristic1, query, ref number);
            number++;
        }
    }

    private static void ApplyId(
        VehicleCharacteristic characteristic,
        IVehiclesSqlQuery query,
        ref int number
    )
    {
        Guid id = characteristic.WhatCharacteristic().Identity.ReadId();
        NpgsqlParameter<Guid> parameter = new($"@modCtxId_{number}", id);
        query.AcceptFilter($"vc.id = @modCtxId_{number}", parameter);
    }

    private static void ApplyName(
        VehicleCharacteristic characteristic,
        IVehiclesSqlQuery query,
        ref int number
    )
    {
        string name = characteristic.WhatCharacteristic().Identity.ReadText();
        NpgsqlParameter<string> parameter = new($"@modCtxName_{number}", name);
        query.AcceptFilter($"vc.text = @modCtxName_{number}", parameter);
    }

    private static void ApplyValue(
        VehicleCharacteristic characteristic,
        IVehiclesSqlQuery query,
        ref int number
    )
    {
        string value = characteristic.WhatValue();
        NpgsqlParameter<string> parameter = new($"@modCtxValue_{number}", value);
        query.AcceptFilter($"pvc.ctx_value = @modCtxValue_{number}", parameter);
    }
}
