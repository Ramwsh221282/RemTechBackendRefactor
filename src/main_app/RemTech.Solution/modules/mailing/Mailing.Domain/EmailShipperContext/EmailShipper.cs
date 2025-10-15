using Mailing.Domain.CommonContext.ValueObjects;
using Mailing.Domain.EmailShipperContext.ValueObjects;

namespace Mailing.Domain.EmailShipperContext;

public sealed class EmailShipper
{
    public EmailShipperId Id { get; }
    public EmailShipperServiceAddress Address { get; } = null!;
    public EmailShipperKey Key { get; } = null!;

    private EmailShipper()
    {
        // ef core
    }

    public EmailShipper(EmailShipperId id, EmailShipperServiceAddress address, EmailShipperKey key)
    {
        Id = id;
        Address = address;
        Key = key;
    }

    public void Subscribe(EmailShippmentProcess shippment)
    {
        ShipperEmailShippmentProcessSubscription subscription = new(Address, Key);
        subscription.Subscribe(shippment);
    }
}
