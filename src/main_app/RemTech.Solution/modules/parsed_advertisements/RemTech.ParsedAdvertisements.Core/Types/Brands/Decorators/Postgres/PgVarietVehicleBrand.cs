using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Postgres;

public sealed class PgVarietVehicleBrand(PgConnectionSource connectionSource, VehicleBrand brand)
    : VehicleBrand(brand)
{
    private readonly Queue<Func<Task<VehicleBrand>>> _strategies = [];

    public async Task<VehicleBrand> SaveAsync(CancellationToken ct = default)
    {
        PgVehicleBrand pgBrand = new(connectionSource, this);
        _strategies.Enqueue(() => pgBrand.SaveAsync(pgBrand.ToStoreCommand(), ct));
        _strategies.Enqueue(() => pgBrand.SaveAsync(pgBrand.FromStoreCommand(), ct));
        while (_strategies.Count > 0)
        {
            try
            {
                Task<VehicleBrand> task = _strategies.Dequeue()();
                return await task;
            }
            catch
            {
                // ignored
            }
        }

        throw new OperationException("Невозможно сохранить бренд");
    }
}
