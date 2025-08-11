namespace Brands.Module.Public;

public interface IBrandsPublicApi
{
    Task<BrandResponse> GetBrand(string name, CancellationToken ct = default);
}
