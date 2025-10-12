using Microsoft.EntityFrameworkCore;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.BrandContext;

public sealed class BrandsRepository : IBrandsDataSource
{
    private readonly VehiclesServiceDbContext _context;

    public BrandsRepository(VehiclesServiceDbContext context) => _context = context;

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
}
