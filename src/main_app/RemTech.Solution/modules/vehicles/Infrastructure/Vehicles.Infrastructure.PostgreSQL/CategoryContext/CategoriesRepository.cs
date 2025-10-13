using Microsoft.EntityFrameworkCore;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.CategoryContext;

public sealed class CategoriesRepository : ICategoryDataSource
{
    private readonly VehiclesServiceDbContext _context;

    public CategoriesRepository(VehiclesServiceDbContext context) => _context = context;

    public async Task<UniqueCategory> GetUnique(CategoryName name, CancellationToken ct = default)
    {
        Category? category = await _context
            .Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken: ct);

        return new UniqueCategory(category?.Name);
    }

    public async Task Add(Category category, CancellationToken ct = default)
    {
        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct);
    }
}
