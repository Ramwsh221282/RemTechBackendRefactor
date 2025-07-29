using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicles(DbDataReader reader)
{
    public async Task<IEnumerable<Vehicle>> Read(CancellationToken ct = default)
    {
        Dictionary<string, Vehicle> vehicles = [];
        while (await reader.ReadAsync(ct))
        {
            Vehicle vehicle = new SqlParsedVehicle(reader).Read();
            if (!vehicles.ContainsKey(vehicle.Id()))
            {
                vehicles.Add(vehicle.Id(), vehicle);
                continue;
            }
            VehicleCharacteristic[] ctxes = vehicle.Characteristics.Read();
            VehicleCharacteristic[] merged =
            [
                .. vehicles[vehicle.Id()].Characteristics.Read(),
                .. ctxes,
            ];
            vehicles[vehicle.Id()] = new Vehicle(vehicles[vehicle.Id()], merged);
        }
        return vehicles.Values.AsEnumerable();
    }
}
