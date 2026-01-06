namespace Identity.Domain.Contracts.Persistence;

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
    
    public AccountTicketSpecification WithLockRequired()
    {
        if (LockRequired.HasValue) return this;
        LockRequired = true;
        return this;
    }

    public AccountTicketSpecification NotFinished()
    {
        if (Finished.HasValue) return this;
        Finished = false;
        return this;
    }
    
    public AccountTicketSpecification WithFinished()
    {
        if (Finished.HasValue) return this;
        Finished = true;
        return this;
    }
}