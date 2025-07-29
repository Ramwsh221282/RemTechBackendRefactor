using System.Data.Common;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicle(DbDataReader reader)
{
    public Vehicle Read()
    {
        SqlParsedVehicleIdentity identity = new(reader);
        SqlParsedVehiclePrice price = new(reader);
        SqlParsedVehiclePhotos photos = new(reader);
        SqlParsedVehicleCharacteristics characteristics = new(reader);
        SqlParsedVehicleModel model = new(reader);
        SqlParsedVehicleKind kind = new(reader);
        SqlParsedVehicleBrand brand = new(reader);
        SqlParsedVehicleRegion region = new(reader);
        Vehicle vehicle = new(identity.Read(), price.Read(), photos.Read());
        vehicle = new Vehicle(vehicle, model.Read());
        vehicle = new Vehicle(vehicle, brand.Read());
        vehicle = new Vehicle(vehicle, region.Read());
        vehicle = new Vehicle(vehicle, kind.Read());
        IEnumerable<VehicleCharacteristic> ctxes = characteristics.Read().Read();
        foreach (VehicleCharacteristic ctx in ctxes)
            vehicle = new Vehicle(vehicle, ctx);
        return vehicle;
    }
}
