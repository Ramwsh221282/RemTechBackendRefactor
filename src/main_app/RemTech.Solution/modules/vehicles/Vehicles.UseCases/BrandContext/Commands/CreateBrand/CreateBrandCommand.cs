using RemTech.UseCases.Shared.Cqrs;
using Vehicles.Domain.BrandContext;

namespace Vehicles.UseCases.BrandContext.Commands.CreateBrand;

public sealed record CreateBrandCommand(string Name) : ICommand<Brand>;
