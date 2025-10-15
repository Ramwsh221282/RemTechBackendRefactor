using Mailing.Domain.CommonContext.ValueObjects;
using Mailing.Domain.EmailShipperContext.ValueObjects;

namespace Mailing.Domain.EmailShipperContext;

public sealed class EmailShipper
{
    public EmailShipperId Id { get; }
    public EmailShipperServiceAddress Address { get; } = null!;
    public EmailShipperKey Key { get; } = null!;
    public EmailShipperActionsCounter Counter { get; private set; }

    private EmailShipper()
    {
        // ef core
    }

    public void ActionCompleted() => Counter = Counter.Increment();

    public EmailShipper(
        EmailShipperId id,
        EmailShipperServiceAddress address,
        EmailShipperKey key,
        EmailShipperActionsCounter counter
    )
    {
        Id = id;
        Address = address;
        Key = key;
        Counter = counter;
    }

    public void Subscribe(EmailShippmentProcess shippment)
    {
        ShipperEmailShippmentProcessSubscription subscription = new(Address, Key);
        subscription.Subscribe(shippment);
    }
}
