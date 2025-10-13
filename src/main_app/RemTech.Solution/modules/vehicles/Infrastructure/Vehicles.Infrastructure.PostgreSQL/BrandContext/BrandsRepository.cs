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

    public async Task<UniqueBrand> GetUniqueBrand(BrandName name, CancellationToken ct = default)
    {
        BrandName? existing = await _context
            .Brands.AsNoTracking()
            .Select(b => b.Name)
            .FirstOrDefaultAsync(n => n == name, cancellationToken: ct);
        return new UniqueBrand(existing);
    }

    public async Task Add(Brand brand, CancellationToken ct = default)
    {
        await _context.Brands.AddAsync(brand, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Brand> GetOrSave(BrandName brandName, CancellationToken ct)
    {
        Pgvector.Vector vector = brandName.CreateEmbedding(_generator);
        Brand brand = await _context
            .Brands.FromSqlInterpolated(
                @$"
             SELECT id,
                    name,
                    rating,
                    vehicles_count
             FROM vehicles_module.brands
             ORDER BY embedding <=> {vector}
             LIMIT 1
            "
            )
            .FirstAsync(cancellationToken: ct);
        return brand;
    }
}
