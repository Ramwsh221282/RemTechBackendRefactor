using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;

public sealed class PgSingleRiddenVehicleBrand(DbDataReader reader)
{
    public async Task<VehicleBrand> Read()
    {
        if (!await reader.ReadAsync())
            throw new ApplicationException("Vehicle brand not found.");
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        return new ExistingVehicleBrand(id, text);
    }
}