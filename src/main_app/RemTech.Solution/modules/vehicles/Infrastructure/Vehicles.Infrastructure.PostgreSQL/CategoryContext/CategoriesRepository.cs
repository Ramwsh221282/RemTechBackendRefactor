using Microsoft.EntityFrameworkCore;
using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.CategoryContext;

public sealed class CategoriesRepository : ICategoryDataSource
{
    private readonly VehiclesServiceDbContext _context;
    private readonly IEmbeddingGenerator _generator;

    public CategoriesRepository(IEmbeddingGenerator generator, VehiclesServiceDbContext dbContext)
    {
        _context = dbContext;
        _generator = generator;
    }

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

    public async Task<Category> GetOrSave(CategoryName name, CancellationToken ct = default)
    {
        Pgvector.Vector vector = name.GenerateEmbedding(_generator);
        return await _context
            .Categories.FromSqlInterpolated(
                @$"
              SELECT
                id,
                name,
                rating,
                vehicles_count
              FROM categories
              ORDER BY embedding <=> {vector}"
            )
            .SingleAsync(cancellationToken: ct);
    }
}
