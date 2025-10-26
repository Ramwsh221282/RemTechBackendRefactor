using RemTech.Core.Shared.Cqrs;

namespace Brands.Module.Features.QueryBrands;

internal sealed record QueryBrandsCommand : ICommand
{
    public string? Name { get; }
    public string? TextSearch { get; }
    public string OrderMode { get; } = "ASC";
    public int Page { get; } = 1;
    public int PageSize { get; } = 20;

    public QueryBrandsCommand(
        string? name,
        string? textSearch,
        string? orderMode,
        int? page,
        int? pageSize
    )
    {
        Name = name;
        TextSearch = textSearch;
        if (!string.IsNullOrEmpty(orderMode) && orderMode == "DESC")
            OrderMode = "DESC";
        Page = page ?? 1;
        PageSize = pageSize ?? 20;
    }
}
