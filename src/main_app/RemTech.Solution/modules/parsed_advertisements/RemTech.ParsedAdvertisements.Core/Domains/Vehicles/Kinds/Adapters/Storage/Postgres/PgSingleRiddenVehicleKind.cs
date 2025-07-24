using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;

public sealed class PgSingleRiddenVehicleKind(DbDataReader reader)
{
    public async Task<VehicleKind> Read()
    {
        if (!await reader.ReadAsync())
            throw new ApplicationException("No vehicle kind found.");
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        return new ExistingVehicleKind(id, text);
    }
}