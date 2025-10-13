using FluentValidation;
using FluentValidation.Results;
using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Cqrs;
using RemTech.UseCases.Shared.Database;
using RemTech.UseCases.Shared.Logging;
using RemTech.UseCases.Shared.Validations;
using Serilog;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.Features.VehicleRegistration;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.UseCases.AddVehicle;

public sealed class AddVehicleCommandHandler : ICommandHandler<AddVehicleCommand, Vehicle>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionSource _transactionSource;
    private readonly IValidator<AddVehicleCommand> _validator;
    private readonly ILogger _logger;
    private readonly IVehicleRegistrator _registrator;

    public AddVehicleCommandHandler(
        ILogger logger,
        IValidator<AddVehicleCommand> validator,
        IUnitOfWork unitOfWork,
        ITransactionSource transactionSource,
        IVehicleRegistrator registrator
    )
    {
        _unitOfWork = unitOfWork;
        _transactionSource = transactionSource;
        _logger = logger;
        _validator = validator;
        _registrator = registrator;
    }

    public async Task<Result<Vehicle>> Handle(
        AddVehicleCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await _validator.ValidateAsync(command, ct);
        if (validation.NotValid())
            return _logger.LoggedError(validation.Failure<Vehicle>(), nameof(AddVehicleCommand));

        await using ITransactionScope txn = await _transactionSource.BeginTransactionScope(ct);

        CategoryName categoryName = CategoryName.Create(command.CategoryName);
        BrandName brandName = BrandName.Create(command.BrandName);
        VehicleModelName modelName = VehicleModelName.Create(command.ModelName);
        LocationAddress address = LocationAddress.Create(command.LocationParts);
        VehicleDescription description = VehicleDescription.Create(command.Description);
        VehiclePrice price = VehiclePrice.Create(command.Price.Value, command.Price.IsNds);
        VehicleCharacteristicsCollection characteristics = VehicleCharacteristicsCollection.Create(
            command.Characteristics.Select(c => VehicleCharacteristic.Create(c.Name, c.Value).Value)
        );
        VehiclePhotosCollection photos = VehiclePhotosCollection.Create(command.PhotoPaths);

        Vehicle vehicle = await _registrator.RegisterVehicle(
            categoryName,
            brandName,
            modelName,
            address,
            description,
            price,
            characteristics,
            photos,
            ct
        );

        Result saving = await _unitOfWork.SaveChanges(ct);
        if (saving.IsFailure)
            return _logger.LoggedError<Vehicle>(saving.Error, nameof(AddVehicleCommand));

        Result commit = await txn.Commit(ct);
        if (commit.IsFailure)
            return _logger.LoggedError<Vehicle>(saving.Error, nameof(AddVehicleCommand));

        return vehicle;
    }
}
