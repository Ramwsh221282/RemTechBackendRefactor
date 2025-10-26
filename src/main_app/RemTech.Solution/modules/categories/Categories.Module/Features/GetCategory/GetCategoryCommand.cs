using RemTech.Core.Shared.Cqrs;

namespace Categories.Module.Features.GetCategory;

internal sealed record GetCategoryCommand(
    Guid? Id = null,
    string? Name = null,
    string? TextSearch = null
) : ICommand;
