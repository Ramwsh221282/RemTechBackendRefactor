namespace Identity.Messaging.Port.EmailTickets;

public sealed class EmailTicketSendResult
{
    public bool IsSuccess { get; } = false;
    public string Message { get; } = string.Empty;
}