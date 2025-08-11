namespace Brands.Module.Features.GetBrand;

internal sealed class BrandRawByNameEmptyNameException : Exception
{
    public BrandRawByNameEmptyNameException()
        : base("Название бренда для получения по имени пустое.") { }

    public BrandRawByNameEmptyNameException(Exception ex)
        : base("Название бренда для получения по имени пустое.", ex) { }
}
