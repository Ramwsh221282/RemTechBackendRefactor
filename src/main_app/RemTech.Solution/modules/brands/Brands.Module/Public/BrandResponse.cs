namespace Brands.Module.Public;

public sealed record BrandResponse(string Name, Guid Id, long Rating)
{
    public static async Task<T> MapTo<T>(
        Func<BrandResponse, T> mapFn,
        Func<Task<BrandResponse>> sourceFn
    )
    {
        BrandResponse brandResponse = await sourceFn();
        return mapFn(brandResponse);
    }
}
