using System.Data.Common;
using RemTech.Core.Shared.Exceptions;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;

public sealed class PgSingleRiddenVehicleBrand(DbDataReader reader)
{
    public async Task<VehicleBrand> Read()
    {
        if (!await reader.ReadAsync())
            throw new OperationException("Бренд не найден.");
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        return new VehicleBrand(
            new VehicleBrandIdentity(new VehicleBrandId(id), new VehicleBrandText(text))
        );
    }
}
