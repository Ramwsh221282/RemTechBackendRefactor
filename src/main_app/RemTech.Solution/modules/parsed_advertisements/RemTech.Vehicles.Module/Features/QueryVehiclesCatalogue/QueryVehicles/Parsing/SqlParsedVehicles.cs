using System.Data.Common;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicles(DbDataReader reader)
{
    public async Task<IEnumerable<Vehicle>> Read(CancellationToken ct = default)
    {
        List<Vehicle> vehicles = [];
        while (await reader.ReadAsync(ct))
        {
            Vehicle vehicle = new SqlParsedVehicle(reader).Read();
            vehicles.Add(vehicle);
        }
        return vehicles;
    }
}
