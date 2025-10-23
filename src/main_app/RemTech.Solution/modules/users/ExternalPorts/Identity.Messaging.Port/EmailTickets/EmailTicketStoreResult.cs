namespace Identity.Messaging.Port.EmailTickets;

public sealed class EmailTicketStoreResult
{
    public string Error { get; } = string.Empty;
    public bool IsSuccess { get; } = false;
}