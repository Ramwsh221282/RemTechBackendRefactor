using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Cqrs;
using RemTech.UseCases.Shared.Database;
using RemTech.UseCases.Shared.Logging;
using Serilog;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.Infrastructure.DataSource;

namespace Vehicles.UseCases.AddVehicle;

public sealed class AddVehicleCommandHandler : ICommandHandler<AddVehicleCommand, Vehicle>
{
    private readonly IVehiclesDataSource _vehicles;
    private readonly IVehicleModelsDataSource _models;
    private readonly ICategoryDataSource _categories;
    private readonly IBrandsDataSource _brands;
    private readonly ILocationsDataSource _locations;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionSource _transactionSource;
    private readonly ILogger _logger;
    private const string CommandName = nameof(AddVehicleCommand);

    public AddVehicleCommandHandler(
        IVehiclesDataSource vehicles,
        IVehicleModelsDataSource models,
        ICategoryDataSource categories,
        IBrandsDataSource brands,
        ILocationsDataSource locations,
        IUnitOfWork unitOfWork,
        ITransactionSource transactionSource,
        ILogger logger
    )
    {
        _vehicles = vehicles;
        _models = models;
        _categories = categories;
        _brands = brands;
        _locations = locations;
        _unitOfWork = unitOfWork;
        _transactionSource = transactionSource;
        _logger = logger;
    }

    public async Task<Result<Vehicle>> Handle(
        AddVehicleCommand command,
        CancellationToken ct = default
    )
    {
        await using ITransactionScope txn = await _transactionSource.BeginTransactionScope(ct);
        Vehicle vehicle = command.ProvideVehicle();

        Result<Vehicle> result = await vehicle.Save(
            _vehicles,
            _models,
            _categories,
            _brands,
            _locations,
            ct
        );

        if (result.IsFailure)
            return _logger.LoggedError(result.Error, CommandName);

        Result saving = await _unitOfWork.SaveChanges(ct);
        if (saving.IsFailure)
            return _logger.LoggedError(saving.Error, CommandName);

        Result commit = await txn.Commit(ct);
        if (commit.IsFailure)
            return _logger.LoggedError(commit.Error, CommandName);

        return result;
    }
}
