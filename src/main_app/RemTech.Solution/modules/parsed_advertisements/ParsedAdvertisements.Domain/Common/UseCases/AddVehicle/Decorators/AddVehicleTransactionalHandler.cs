using System.Text.Json;
using ParsedAdvertisements.Domain.VehicleContext;
using ParsedAdvertisements.Domain.VehicleContext.Events;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain.Common.UseCases.AddVehicle.Decorators;

public sealed class AddVehicleTransactionalHandler(
    ICommandHandler<AddVehicleCommand, Status<Vehicle>> handler,
    IParsedAdvertisementsOutboxDeliverer deliverer,
    ITransactionManager manager)
    : ICommandHandler<AddVehicleCommand, Status<Vehicle>>
{
    public async Task<Status<Vehicle>> Handle(AddVehicleCommand command, CancellationToken ct = default)
    {
        await manager.Begin(ct);
        var status = await handler.Handle(command, ct);
        if (status.IsFailure)
            return status.Error;

        var message = CreateOutboxMessage(command);
        await deliverer.Save(message, manager, ct);
        var commit = await manager.Commit(ct);

        return commit.IsFailure ? commit.Error : status;
    }

    private ParsedAdvertisementsOutboxMessage CreateOutboxMessage(AddVehicleCommand command)
    {
        var @event = new VehicleCreatedEvent(
            command.VehicleId,
            "VEHICLE",
            command.SourceDomain,
            command.SourceUrl,
            command.SourceId);

        var type = nameof(VehicleCreatedEvent);
        var content = JsonSerializer.Serialize(@event);

        return new ParsedAdvertisementsOutboxMessage()
        {
            Id = Guid.NewGuid(),
            Content = content,
            Created = DateTime.UtcNow,
            LastError = null,
            Retries = 0,
            Type = type,
            Status = ParsedAdvertisementsOutboxMessage.Pending
        };
    }
}