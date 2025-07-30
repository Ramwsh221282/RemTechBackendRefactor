using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public sealed class VehicleCharacteristicQuerySpecification(IEnumerable<VehicleCharacteristic> characteristic)
  : IQueryVehiclesSpecification
{
  public void ApplyTo(VehiclesSqlQuery query)
  {
    int number = 1;
    foreach (VehicleCharacteristic characteristic1 in characteristic)
    {
      Guid id = characteristic1.WhatCharacteristic().Identity.ReadId();
      string name = characteristic1.WhatCharacteristic().Identity.ReadText();
      string value = characteristic1.WhatValue();
      NpgsqlParameter[] parameters =
      [
        new NpgsqlParameter<Guid>($"@modCtxId_{number}", id),
        new NpgsqlParameter<string>($"@modCtxName_{number}", name),
        new NpgsqlParameter<string>($"@modCtxValue_{number}", value),
      ];
      query.AcceptFilter(
        string.Intern(
          $"""
           EXISTS (
           SELECT 1 FROM parsed_advertisements_module.parsed_vehicle_characteristics sub_ctx
           WHERE sub_ctx.vehicle_id = v.id 
           AND sub_ctx.ctx_id = @modCtxId_{number}
           AND sub_ctx.ctx_name = @modCtxName_{number}
           AND sub_ctx.ctx_value = @modCtxValue_{number}
           )
           """
        ),
        parameters
      );
      number++;
    }
  }
}