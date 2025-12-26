using Categories.Module.Features.GetCategory;
using Categories.Module.Responses;
using Categories.Module.Types;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Categories.Module.Public;

internal sealed class GetCategoryApi(ICommandHandler<GetCategoryCommand, Status<ICategory>> handler)
    : IGetCategoryApi
{
    public async Task<Status<CategoryResponse>> GetCategory(
        Guid? id = null,
        string? name = null,
        string? textSearch = null,
        CancellationToken ct = default
    )
    {
        var command = new GetCategoryCommand(id, name, textSearch);
        var result = await handler.Handle(command, ct);

        return result.IsFailure
            ? result.Error
            : new CategoryResponse(result.Value.Id, result.Value.Name, result.Value.Rating);
    }
}
