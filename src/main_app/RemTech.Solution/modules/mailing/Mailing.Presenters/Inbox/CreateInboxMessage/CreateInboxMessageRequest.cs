namespace Mailing.Presenters.Inbox.CreateInboxMessage;

public sealed record CreateInboxMessageRequest(
    string TargetEmail, 
    string Subject, 
    string Body, 
    CancellationToken Ct)
    : IRequest;