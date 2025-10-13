namespace Vehicles.Domain.BrandContext.Infrastructure.DataSource;

public interface IBrandsDataSource
{
    Task<Brand> Add(Brand brand, CancellationToken ct = default);
    Task<Brand> GetOrSave(Brand brand, CancellationToken ct);
}
