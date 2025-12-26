using RemTech.Core.Shared.Result;

namespace Brands.Module.Public.GetBrand;

public interface IGetBrandApi
{
    Task<Status<BrandResponse>> GetBrand(
        Guid? id = null,
        string? name = null,
        string? textSearch = null,
        CancellationToken ct = default
    );
}
