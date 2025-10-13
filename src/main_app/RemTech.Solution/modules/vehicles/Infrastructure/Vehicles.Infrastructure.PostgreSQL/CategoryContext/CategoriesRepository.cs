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

    public async Task<Category> Add(Category category, CancellationToken ct = default)
    {
        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct);
        return category;
    }

    public async Task<Category> GetOrSave(Category category, CancellationToken ct = default)
    {
        Category? relevant = await GetRelevantByName(category.Name, ct);
        return relevant ?? await Add(category, ct);
    }

    private async Task<Category?> GetRelevantByName(
        CategoryName name,
        CancellationToken ct = default
    )
    {
        Pgvector.Vector vector = name.GenerateEmbedding(_generator);
        return await _context
            .Categories.FromSqlInterpolated(
                @$"
              SELECT
                id,
                name,
                rating,
                vehicles_count,
                embedding
              FROM vehicles_module.categories
              ORDER BY embedding <=> {vector}
              LIMIT 1"
            )
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
