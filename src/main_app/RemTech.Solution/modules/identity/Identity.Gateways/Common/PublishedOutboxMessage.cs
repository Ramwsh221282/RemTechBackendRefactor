using Identity.Infrastructure.Outbox;

namespace Identity.Gateways.Common;

public sealed record PublishedOutboxMessage(IdentityOutboxMessage Message)
{
    public IdentityOutboxMessage Origin()
    {
        return Message;
    }
}