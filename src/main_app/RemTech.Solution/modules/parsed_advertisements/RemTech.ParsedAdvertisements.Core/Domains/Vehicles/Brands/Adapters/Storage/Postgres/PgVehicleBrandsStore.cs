using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;

public sealed class PgVehicleBrandsStore(PgConnectionSource connectionSource) : IPgVehicleBrandsStorage
{
    public async Task<VehicleBrand> Get(VehicleBrand brand, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await brand.StoreCommand().Execute(connection, ct) != 1
            ? throw new ApplicationException($"Unable to store brand. Duplicate by name: {brand.Identify().ReadText()}")
            : brand;
    }
}