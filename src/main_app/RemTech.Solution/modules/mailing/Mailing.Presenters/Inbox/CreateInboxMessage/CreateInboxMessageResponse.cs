namespace Mailing.Presenters.Inbox.CreateInboxMessage;

public sealed record CreateInboxMessageResponse(
    string TargetEmail, 
    string Subject, 
    string Body) 
    : IResponse;