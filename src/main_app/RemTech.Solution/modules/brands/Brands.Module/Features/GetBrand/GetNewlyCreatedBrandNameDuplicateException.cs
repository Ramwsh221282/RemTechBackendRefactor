namespace Brands.Module.Features.GetBrand;

internal sealed class GetNewlyCreatedBrandNameDuplicateException : Exception
{
    public GetNewlyCreatedBrandNameDuplicateException(string name)
        : base($"Дубликат бренда по имени: {name}") { }

    public GetNewlyCreatedBrandNameDuplicateException(string name, Exception ex)
        : base($"Дубликат бренда по имени: {name}", ex) { }
}
