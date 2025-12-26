using Mailing.Core.Inbox;

namespace Mailing.Infrastructure.InboxMessageProcessing.Protocols;

public interface MarkProcessedMessagesProtocol
{
    Task Mark(IEnumerable<InboxMessage> messages, CancellationToken ct);
}