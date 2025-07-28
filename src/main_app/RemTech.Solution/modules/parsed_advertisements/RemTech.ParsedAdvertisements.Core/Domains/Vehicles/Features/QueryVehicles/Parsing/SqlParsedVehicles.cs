using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Parsing;

public sealed class SqlParsedVehicles(DbDataReader reader)
{
    public async Task<IEnumerable<Vehicle>> Read(CancellationToken ct = default)
    {
        List<Vehicle> vehicles = [];
        while (await reader.ReadAsync(ct))
            vehicles.Add(new SqlParsedVehicle(reader).Read());
        return vehicles;
    }
}