using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using Tickets.Adapter.Storage.DataModels;
using Tickets.Adapter.Storage.Implementations;
using Tickets.Domain.Tickets.Events;

namespace Tickets.Adapter.Storage.EventHandlers;

public sealed class TicketCreatedEventHandler(Serilog.ILogger logger, TicketsStorage storage)
    : IDomainEventHandler<TicketCreatedEvent>
{
    private const string Context = nameof(TicketCreatedEventHandler);

    public async Task<Status> Handle(TicketCreatedEvent @event, CancellationToken ct = default)
    {
        try
        {
            TicketDataModel dm = ToDataModel(@event);
            Status adding = await storage.Add(dm, ct);
            return adding.IsFailure ? Status.Failure(adding.Error) : Status.Success();
        }
        catch (Exception ex)
        {
            logger.Error("{Context}. Error: {ErrorMessage}.", Context, ex);
            return Status.Internal("Ошибка при сохранении заявки.");
        }
    }

    private TicketDataModel ToDataModel(TicketCreatedEvent @event) =>
        new()
        {
            Id = @event.Id,
            Content = @event.Content,
            Created = @event.Created,
            Deleted = null,
        };
}
