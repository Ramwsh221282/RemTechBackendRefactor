using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Domain.BrandContext.Infrastructure.DataSource;

public interface IBrandsDataSource
{
    Task<UniqueBrand> GetUniqueBrand(BrandName name, CancellationToken ct = default);
    Task Add(Brand brand, CancellationToken ct = default);
    Task<Brand> GetOrSave(BrandName brandName, CancellationToken ct);
}
