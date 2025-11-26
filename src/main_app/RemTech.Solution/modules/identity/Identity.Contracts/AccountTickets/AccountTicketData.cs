namespace Identity.Contracts.AccountTickets;

public sealed record AccountTicketData(
    Guid Id,
    Guid AccountId,
    string Type,
    string Payload,
    DateTime Created,
    DateTime? Finished
);