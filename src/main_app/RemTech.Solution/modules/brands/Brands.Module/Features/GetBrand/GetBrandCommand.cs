using Shared.Infrastructure.Module.Cqrs;

namespace Brands.Module.Features.GetBrand;

internal sealed record GetBrandCommand(string Name) : ICommand;
