using FluentValidation;
using ParsedAdvertisements.Domain.VehicleContext;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace ParsedAdvertisements.Domain.Common.UseCases.AddVehicle.Decorators;

public sealed class AddVehicleValidationalHandler(
    ICommandHandler<AddVehicleCommand, Status<Vehicle>> handler,
    IValidator<AddVehicleCommand> validator)
    : ICommandHandler<AddVehicleCommand, Status<Vehicle>>
{
    public async Task<Status<Vehicle>> Handle(AddVehicleCommand command, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(command, ct);
        return !validation.IsValid ? validation.ValidationError() : await handler.Handle(command, ct);
    }
}