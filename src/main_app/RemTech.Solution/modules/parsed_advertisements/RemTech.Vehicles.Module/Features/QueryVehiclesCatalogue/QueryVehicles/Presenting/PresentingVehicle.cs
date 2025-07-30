using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;
using RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;
using RemTech.Vehicles.Module.Types.Transport;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;

public sealed class PresentingVehicle(Vehicle origin) : Vehicle(origin)
{
    public VehiclePresentation Present()
    {
        VehicleIdentityPresentation identity = new(Identity.Read());
        VehicleKindPresentation kind = new(Kind.Id(), Kind.Name());
        VehicleBrandPresentation brand = new(Brand.Id(), Brand.Name());
        VehicleModelPresentation model = new(Model.Id(), Model.NameString());
        VehicleRegionPresentation region = new(Location.Id(), Location.Name(), Location.Kind());
        VehiclePricePresentation price = new(Price.Value(), Price.UnderNds());
        VehiclePhotosPresentation photos = new(
            Photos.Read().Select(p => new VehiclePhotoPresentation(p))
        );
        VehicleCharacteristicsPresentation characteristic = new(
            Characteristics
                .Read()
                .Select(c => new VehicleCharacteristicPresentation(
                    Identity.Read(),
                    c.WhatCharacteristic().Identity.ReadId(),
                    c.WhatCharacteristic().Identity.ReadText(),
                    c.WhatValue(),
                    c.WhatCharacteristic().Measure()
                ))
        );
        return new VehiclePresentation(
            identity,
            kind,
            brand,
            model,
            region,
            price,
            photos,
            characteristic
        );
    }
}
