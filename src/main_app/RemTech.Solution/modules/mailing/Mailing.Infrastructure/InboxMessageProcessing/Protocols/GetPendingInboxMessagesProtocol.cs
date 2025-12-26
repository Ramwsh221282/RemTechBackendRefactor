using Mailing.Core.Inbox;

namespace Mailing.Infrastructure.InboxMessageProcessing.Protocols;

public interface GetPendingInboxMessagesProtocol
{
    IAsyncEnumerable<InboxMessage> GetPendingMessages(CancellationToken ct);
}