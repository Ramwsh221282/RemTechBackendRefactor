using Mailing.Core.Inbox;

namespace Mailing.Infrastructure.InboxMessageProcessing.Protocols;

public interface RemoveManyInboxMessagesProtocol
{
    Task Remove(IEnumerable<InboxMessage> messages, CancellationToken ct);
}