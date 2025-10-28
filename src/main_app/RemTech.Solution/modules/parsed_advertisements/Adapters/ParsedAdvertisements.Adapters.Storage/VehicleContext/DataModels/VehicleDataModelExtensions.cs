using System.Text.Json;
using ParsedAdvertisements.Domain.VehicleContext;

namespace ParsedAdvertisements.Adapters.Storage.VehicleContext.DataModels;

public static class VehicleDataModelExtensions
{
    public static VehicleDataModel ToDataModel(this Vehicle vehicle)
    {
        return new VehicleDataModel
        {
            VehicleId = vehicle.Identity.VehicleId,
            BrandId = vehicle.Identity.BrandId,
            CategoryId = vehicle.Identity.CategoryId,
            ModelId = vehicle.Identity.ModelId,
            LocationId = vehicle.Identity.LocationId,
            Price = vehicle.Price.Value,
            IsNds = vehicle.Price.IsNds,
            Domain = vehicle.Source.Domain,
            Url = vehicle.Source.Url,
            LocationPath = vehicle.Path.Path,
            Photos = JsonSerializer.Serialize(vehicle.Photos),
            Characteristics = vehicle.Characteristics.Characteristic.Select(c => new VehicleCharacteristicDataModel()
            {
                VehicleId = c.VehicleId,
                CharacteristicId = c.CharacteristicId,
                CharacteristicName = c.CharacteristicName,
                CharacteristicValue = c.CharacteristicValue
            })
        };
    }
}