using Microsoft.EntityFrameworkCore;
using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.ModelContext;

public sealed class VehicleModelsRepository : IVehicleModelsDataSource
{
    private readonly VehiclesServiceDbContext _context;
    private readonly IEmbeddingGenerator _generator;

    public VehicleModelsRepository(VehiclesServiceDbContext context, IEmbeddingGenerator generator)
    {
        _context = context;
        _generator = generator;
    }

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

    public async Task<VehicleModel> GetOrSave(VehicleModelName modelName, CancellationToken ct)
    {
        Pgvector.Vector vector = modelName.Generate(_generator);
        return await _context
            .Models.FromSqlInterpolated(
                $@"
                SELECT  id,
                        name,
                        vehicles_count,
                        rating
                FROM models
                ORDER BY embedding <=> {vector}
                LIMIT 1
                "
            )
            .FirstAsync(cancellationToken: ct);
    }
}
