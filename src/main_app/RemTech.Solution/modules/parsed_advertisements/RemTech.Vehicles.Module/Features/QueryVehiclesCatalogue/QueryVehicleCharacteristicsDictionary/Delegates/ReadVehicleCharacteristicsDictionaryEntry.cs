using System.Data.Common;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Delegates;

public delegate VehicleCharacteristicsDictionaryEntry ReadVehicleCharacteristicsDictionaryEntry(
    DbDataReader reader
);
