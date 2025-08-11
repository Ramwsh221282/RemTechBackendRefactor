namespace Brands.Module.Features.GetBrand;

internal sealed class BrandRawByNameNotFoundException : Exception
{
    public BrandRawByNameNotFoundException(string name)
        : base($"Не удается найти бренд по имени {name}") { }

    public BrandRawByNameNotFoundException(string name, Exception ex)
        : base($"Не удается найти бренд по имени {name}", ex) { }
}
