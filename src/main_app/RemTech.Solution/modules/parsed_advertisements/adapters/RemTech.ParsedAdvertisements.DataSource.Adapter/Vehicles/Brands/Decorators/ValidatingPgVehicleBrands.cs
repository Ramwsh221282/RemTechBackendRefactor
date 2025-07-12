using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands.Decorators;

public sealed class ValidatingPgVehicleBrands : IAsyncVehicleBrands
{
    private readonly IAsyncVehicleBrands _origin;

    public ValidatingPgVehicleBrands(IAsyncVehicleBrands origin)
    {
        _origin = origin;
    }

    public Task<Status<IVehicleBrand>> Add(IVehicleBrand brand, CancellationToken ct = default)
    {
        if (!brand.Identify().ReadId())
            return new ValidationError<IVehicleBrand>("Некорректный идентификатор бренда.");
        if (!brand.Identify().ReadText())
            return new ValidationError<IVehicleBrand>("Некорректное название бренда.");
        return _origin.Add(brand, ct);
    }

    public Task<MaybeBag<IVehicleBrand>> Find(
        VehicleBrandIdentity identity,
        CancellationToken ct = default
    )
    {
        if (!identity.ReadId() && !identity.ReadText())
            return Task.FromResult(new MaybeBag<IVehicleBrand>());
        return _origin.Find(identity, ct);
    }

    public void Dispose()
    {
        _origin.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _origin.DisposeAsync();
    }
}
