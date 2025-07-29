using System.Data.Common;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

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
