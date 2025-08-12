using Shared.Infrastructure.Module.Cqrs;

namespace Brands.Module.Features.QueryBrands;

internal sealed record QueryBrandsCommand(int Page, string? Text = null) : ICommand;
