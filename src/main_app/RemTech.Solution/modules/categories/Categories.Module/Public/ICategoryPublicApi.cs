namespace Categories.Module.Public;

public interface ICategoryPublicApi
{
    Task<CategoryResponse> GetCategory(string name, CancellationToken ct = default);
}
