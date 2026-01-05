using Identity.Domain.Accounts.Models;
using Identity.Domain.Accounts.Models.Events;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

public sealed class OnNewAccountCreatedEventHandler(Serilog.ILogger logger, IAccountTicketsRepository tickets, IAccountModuleOutbox outbox)
    : IDomainEventHandler, IDomainEventHandler<NewAccountCreatedEvent>
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<OnNewAccountCreatedEventHandler>();
    private IAccountTicketsRepository Tickets { get; } = tickets;
    private IAccountModuleOutbox Outbox { get; } = outbox;
    
    public async Task Handle(IDomainEvent @event, CancellationToken ct = new CancellationToken())
    {
        if (@event is NewAccountCreatedEvent newAccountCreatedEvent) await Handle(newAccountCreatedEvent, ct);
        else Logger.Warning("Domain event is of type {Name}. Skipping.", @event.GetType().Name);
    }

    public async Task Handle(NewAccountCreatedEvent @event, CancellationToken ct = new CancellationToken())
    {
        AccountTicket ticket = CreateConfirmEmailTicket(@event);
        OutboxMessage message = CreateOutboxMessage(ticket, @event);
        await Tickets.Add(ticket, ct);
        await Outbox.Add(message, ct);
        Logger.Information("Handled event of type {Type}", @event.GetType().Name);
    }

    private OutboxMessage CreateOutboxMessage(AccountTicket ticket, NewAccountCreatedEvent @event)
    {
        NewAccountRegisteredOutboxMessagePayload payload = new(
            @event.AccountId,
            ticket.TicketId,
            @event.AccountEmail,
            @event.AccountLogin);
        
        OutboxMessage message = OutboxMessage.CreateNew(AccountOutboxMessageTypes.NewAccountCreated, payload);
        
        Logger.Information("Created outbox message of type: {Type}.", message.Type);
        Logger.Debug("Outbox message payload: {AccountId} {TicketId} {AccountEmail} {AccountLogin}", 
            payload.AccountId, 
            payload.TicketId, 
            payload.Email, 
            payload.Login);
        
        return message;
    }
    
    private AccountTicket CreateConfirmEmailTicket(NewAccountCreatedEvent @event)
    {
        AccountTicket ticket = AccountTicket.New(@event.AccountId, AccountTicketPurposes.EmailConfirmationRequired);
        Logger.Information("Created email confirmation ticket for account {AccountId}", @event.AccountId);
        return ticket;
    }
}