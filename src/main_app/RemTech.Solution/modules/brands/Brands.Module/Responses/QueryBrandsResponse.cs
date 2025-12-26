namespace Brands.Module.Responses;

internal sealed record QueryBrandsResponse(long Count, IEnumerable<BrandDto> Brands);
