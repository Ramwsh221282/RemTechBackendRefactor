namespace Mailing.Core.Inbox.Protocols;

public interface SaveInboxMessageProtocol
{
    Task Save(InboxMessage message, CancellationToken ct);
}