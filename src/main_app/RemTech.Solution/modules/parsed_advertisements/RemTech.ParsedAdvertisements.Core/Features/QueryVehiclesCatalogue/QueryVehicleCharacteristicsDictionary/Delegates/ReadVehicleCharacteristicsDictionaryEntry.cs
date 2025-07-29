﻿using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Delegates;

public delegate VehicleCharacteristicsDictionaryEntry ReadVehicleCharacteristicsDictionaryEntry(
    DbDataReader reader
);
