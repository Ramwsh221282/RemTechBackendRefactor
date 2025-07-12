using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands.Decorators;

public sealed class LoggingPgVehicleBrands : IAsyncVehicleBrands
{
    private readonly ICustomLogger _logger;
    private readonly IAsyncVehicleBrands _origin;

    public LoggingPgVehicleBrands(ICustomLogger logger, IAsyncVehicleBrands origin)
    {
        _logger = logger;
        _origin = origin;
    }

    public async Task<Status<IVehicleBrand>> Add(
        IVehicleBrand brand,
        CancellationToken ct = default
    )
    {
        Status<IVehicleBrand> status = await _origin.Add(brand, ct);
        if (status.IsSuccess)
        {
            _logger.Info("В БД записан новый бренд:");
            _logger.Info("ID: {0}.", (Guid)status.Value.Identify().ReadId());
            _logger.Info("Название: {0}.", (string)status.Value.Identify().ReadText());
            return status;
        }
        _logger.Error("Ошибка добавления бренда в БД: {0}.", status.Error.ErrorText);
        return status;
    }

    public async Task<MaybeBag<IVehicleBrand>> Find(
        VehicleBrandIdentity identity,
        CancellationToken ct = default
    )
    {
        MaybeBag<IVehicleBrand> maybe = await _origin.Find(identity, ct);
        if (maybe.Any())
        {
            _logger.Info("Из базы данных найден бренд по требованию:");
            _logger.Info("ID: {0}.", (Guid)identity.ReadId());
            _logger.Info("Название: {0}.", (string)identity.ReadText());
            return maybe;
        }
        _logger.Warn("Бренд по требованию не найден.");
        _logger.Warn("ID: {0}.", (Guid)identity.ReadId());
        _logger.Warn("Название: {0}.", (string)identity.ReadText());
        return maybe;
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
