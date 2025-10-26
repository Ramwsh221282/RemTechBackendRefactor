using RemTech.Core.Shared.Cqrs;

namespace Brands.Module.Features.GetBrand;

internal sealed record GetBrandCommand(
    Guid? Id = null,
    string? Name = null,
    string? TextSearch = null
) : ICommand;
