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

    public async Task<VehicleModel> Add(VehicleModel model, CancellationToken ct = default)
    {
        await _context.AddAsync(model, ct);
        await _context.SaveChangesAsync(ct);
        return model;
    }

    public async Task<VehicleModel> GetOrSave(VehicleModel model, CancellationToken ct)
    {
        VehicleModel? relevant = await GetRelevantByName(model.Name, ct);
        return relevant ?? await Add(model, ct);
    }

    private async Task<VehicleModel?> GetRelevantByName(VehicleModelName name, CancellationToken ct)
    {
        Pgvector.Vector vector = name.Generate(_generator);
        return await _context
            .Models.FromSqlInterpolated(
                $@"
                SELECT  id,
                        name,
                        vehicles_count,
                        rating,
                        embedding
                FROM vehicles_module.models
                ORDER BY embedding <=> {vector}
                LIMIT 1
                "
            )
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
