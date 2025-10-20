using RemTech.Core.Shared.Cqrs;

namespace Brands.Module.Features.GetBrand;

internal sealed record GetBrandCommand(string Name) : ICommand;
