using Brands.Module.Features.GetBrand;
using Brands.Module.Types;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Brands.Module.Public.GetBrand;

internal sealed class GetBrandApi(ICommandHandler<GetBrandCommand, Status<IBrand>> handler)
    : IGetBrandApi
{
    public async Task<Status<BrandResponse>> GetBrand(
        Guid? id = null,
        string? name = null,
        string? textSearch = null,
        CancellationToken ct = default
    )
    {
        var command = new GetBrandCommand(id, name, textSearch);
        var brand = await handler.Handle(command, ct);
        return brand.IsFailure
            ? brand.Error
            : new BrandResponse(brand.Value.Name, brand.Value.Id, brand.Value.Rating);
    }
}
