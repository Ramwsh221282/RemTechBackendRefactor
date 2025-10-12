using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Domain.CategoryContext.Infrastructure.DataSource;

public interface ICategoryDataSource
{
    Task<UniqueCategory> GetUnique(CategoryName name, CancellationToken ct = default);
    Task Add(Category category, CancellationToken ct = default);
}
