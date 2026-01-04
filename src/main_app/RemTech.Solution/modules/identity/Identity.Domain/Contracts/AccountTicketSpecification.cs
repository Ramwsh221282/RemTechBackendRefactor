namespace Identity.Domain.Contracts;

public sealed class AccountTicketSpecification
{
    public Guid? AccountId { get; private set; }
    public Guid? TicketId { get; private set; }
    public string? Purpose { get; private set; }
    public bool? Finished { get; private set; }
    public bool? LockRequired { get; private set; }

    public AccountTicketSpecification WithAccountId(Guid accountId)
    {
        if (AccountId.HasValue) return this;
        AccountId = accountId;
        return this;
    }
    
    public AccountTicketSpecification WithTicketId(Guid ticketId)
    {
        if (TicketId.HasValue) return this;
        TicketId = ticketId;
        return this;
    }
    
    public AccountTicketSpecification WithPurpose(string purpose)
    {
        if (!string.IsNullOrWhiteSpace(Purpose)) return this;
        Purpose = purpose;
        return this;
    }
    
    public AccountTicketSpecification WithLockRequired(bool lockRequired)
    {
        if (LockRequired.HasValue) return this;
        LockRequired = lockRequired;
        return this;
    }
    
    public AccountTicketSpecification WithFinished(bool finished)
    {
        if (Finished.HasValue) return this;
        Finished = finished;
        return this;
    }
}