namespace Categories.Module.Public;

public sealed record CategoryResponse(Guid Id, string Name, long Rating)
{
    public static async Task<T> MapTo<T>(
        Func<CategoryResponse, T> mapFn,
        Func<Task<CategoryResponse>> sourceFn
    )
    {
        CategoryResponse catResponse = await sourceFn();
        return mapFn(catResponse);
    }
}
