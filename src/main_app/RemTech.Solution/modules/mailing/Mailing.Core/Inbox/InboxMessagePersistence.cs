using Mailing.Core.Inbox.Protocols;

namespace Mailing.Core.Inbox;

public static class InboxMessagePersistence
{
    extension(InboxMessage message)
    {
        public async Task Save(SaveInboxMessageProtocol protocol, CancellationToken ct)
        {
            await protocol.Save(message, ct);
        }
    }
}