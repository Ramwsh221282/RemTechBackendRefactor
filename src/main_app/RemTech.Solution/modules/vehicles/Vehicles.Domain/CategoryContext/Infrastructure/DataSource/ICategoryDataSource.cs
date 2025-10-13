namespace Vehicles.Domain.CategoryContext.Infrastructure.DataSource;

public interface ICategoryDataSource
{
    Task<Category> Add(Category category, CancellationToken ct = default);
    Task<Category> GetOrSave(Category category, CancellationToken ct = default);
}
