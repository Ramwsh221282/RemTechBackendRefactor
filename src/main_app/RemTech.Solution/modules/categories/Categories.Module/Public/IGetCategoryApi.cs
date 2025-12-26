using Categories.Module.Responses;
using RemTech.Core.Shared.Result;

namespace Categories.Module.Public;

public interface IGetCategoryApi
{
    Task<Status<CategoryResponse>> GetCategory(
        Guid? id = null,
        string? name = null,
        string? textSearch = null,
        CancellationToken ct = default
    );
}
