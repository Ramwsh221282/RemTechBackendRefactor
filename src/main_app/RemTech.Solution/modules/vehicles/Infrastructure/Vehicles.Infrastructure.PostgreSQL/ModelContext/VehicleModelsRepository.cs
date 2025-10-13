using Microsoft.EntityFrameworkCore;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.ModelContext;

public sealed class VehicleModelsRepository : IVehicleModelsDataSource
{
    private readonly VehiclesServiceDbContext _context;

    public VehicleModelsRepository(VehiclesServiceDbContext context) => _context = context;

    public async Task Add(VehicleModel model, CancellationToken ct = default)
    {
        await _context.AddAsync(model, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<UniqueVehicleModel> GetUnique(
        VehicleModelName name,
        CancellationToken ct = default
    )
    {
        VehicleModel? model = await _context
            .Models.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Name == name, cancellationToken: ct);

        return new UniqueVehicleModel(model?.Name);
    }
}
