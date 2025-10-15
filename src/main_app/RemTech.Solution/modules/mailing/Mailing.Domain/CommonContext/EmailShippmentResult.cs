using Mailing.Domain.CommonContext.ValueObjects;
using Mailing.Domain.EmailShipperContext;
using Mailing.Domain.EmailShipperContext.ValueObjects;
using Mailing.Domain.EmailShippmentContext;
using Mailing.Domain.EmailShippmentContext.ValueObjects;

namespace Mailing.Domain.CommonContext;

public sealed record EmailShippmentResult
{
    public EmailShipperId ShipperId { get; init; }
    public EmailAddress From { get; init; }
    public EmailAddress To { get; init; }
    public EmailShipperActionsCounter Counter { get; }
    public EmailDestinationDetails Destination { get; init; }
    public EmailMessageDetails Message { get; init; }
    public EmailShippmentId Id { get; init; }
    public EmailShippmentDate ShippedDate { get; init; }

    public EmailShippmentResult(EmailShipper shipper, EmailShippment shippment)
    {
        From = shipper.Address.Address;
        To = shipper.Address.Address;
        Counter = shipper.Counter;
        ShipperId = shipper.Id;
        Destination = shippment.Destination;
        Message = shippment.Message;
        Id = shippment.Id;
        ShippedDate = shippment.Date;
    }
}
