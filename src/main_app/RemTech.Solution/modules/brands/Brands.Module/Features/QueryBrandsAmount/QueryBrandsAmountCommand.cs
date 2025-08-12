using Shared.Infrastructure.Module.Cqrs;

namespace Brands.Module.Features.QueryBrandsAmount;

internal sealed record QueryBrandsAmountCommand(string? Text = null) : ICommand;
