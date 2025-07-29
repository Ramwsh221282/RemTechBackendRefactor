using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Delegates;

public delegate Task<VehicleCharacteristicsDictionary> ReadVehicleCharacteristicsDictionary(
    DbDataReader reader,
    CancellationToken ct = default
);
