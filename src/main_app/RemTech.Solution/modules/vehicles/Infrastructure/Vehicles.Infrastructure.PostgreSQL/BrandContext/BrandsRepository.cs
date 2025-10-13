using Microsoft.EntityFrameworkCore;
using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.BrandContext;

public sealed class BrandsRepository : IBrandsDataSource
{
    private readonly VehiclesServiceDbContext _context;
    private readonly IEmbeddingGenerator _generator;

    public BrandsRepository(VehiclesServiceDbContext context, IEmbeddingGenerator generator)
    {
        _context = context;
        _generator = generator;
    }

    public async Task<Brand> Add(Brand brand, CancellationToken ct = default)
    {
        await _context.Brands.AddAsync(brand, ct);
        await _context.SaveChangesAsync(ct);
        return brand;
    }

    public async Task<Brand> GetOrSave(Brand brand, CancellationToken ct)
    {
        Brand? relevant = await GetRelevantByName(brand.Name, ct);
        return relevant ?? await Add(brand, ct);
    }

    private async Task<Brand?> GetRelevantByName(BrandName name, CancellationToken ct)
    {
        Pgvector.Vector vector = name.CreateEmbedding(_generator);
        return await _context
            .Brands.FromSqlInterpolated(
                @$"
             SELECT id,
                    name,
                    rating,
                    vehicles_count,
                    embedding
             FROM vehicles_module.brands
             ORDER BY embedding <=> {vector}
             LIMIT 1
            "
            )
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
