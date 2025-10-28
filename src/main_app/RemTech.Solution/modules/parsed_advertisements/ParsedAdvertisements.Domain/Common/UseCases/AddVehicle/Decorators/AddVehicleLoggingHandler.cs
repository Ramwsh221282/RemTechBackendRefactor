using ParsedAdvertisements.Domain.VehicleContext;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.Common.UseCases.AddVehicle.Decorators;

public sealed class AddVehicleLoggingHandler(
    ICommandHandler<AddVehicleCommand, Status<Vehicle>> handler,
    Serilog.ILogger logger) : ICommandHandler<AddVehicleCommand, Status<Vehicle>>
{
    private const string Context = nameof(AddVehicleCommand);

    public async Task<Status<Vehicle>> Handle(AddVehicleCommand command, CancellationToken ct = default)
    {
        logger.Information("{Context} handle invoked.", Context);
        var status = await handler.Handle(command, ct);
        logger.Information("{Context} handle finished.", Context);

        if (status.IsFailure)
        {
            logger.Error("{Context} unable to add vehicle.", Context);
        }
        else
        {
            logger.Information(
                "{Context} vehicle created. ID - {ID}. URL: {Url}",
                Context,
                command.VehicleId,
                command.SourceUrl);
        }

        return status;
    }
}