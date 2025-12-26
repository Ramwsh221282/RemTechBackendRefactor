using Mailing.Core.Inbox;

namespace Mailing.Core.Mailers.Protocols;

public interface MessageDeliveryProtocol
{
    Task<DeliveredMessage> Process(Mailer mailer, InboxMessage message, CancellationToken ct);
}