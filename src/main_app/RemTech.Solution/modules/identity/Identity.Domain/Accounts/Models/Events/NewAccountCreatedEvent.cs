using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Models.Events;

public sealed class NewAccountCreatedEvent(Guid accountId, string accountEmail, string accountLogin) : IDomainEvent
{
    public NewAccountCreatedEvent(Account account) : this(account.Id.Value, account.Email.Value, account.Login.Value) { }
    
    public Guid AccountId { get; private set; } = accountId;
    public string AccountEmail { get; private set; } = accountEmail;
    public string AccountLogin { get; private set; } = accountLogin;
    
    public async Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default) => await handler.Handle(this, ct);
}