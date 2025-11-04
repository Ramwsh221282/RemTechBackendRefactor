namespace Mailing.Domain.Postmans;

public interface IMessageTransport
{
    void Send(MessageDeliveryContext context, AsyncDelayedExecutionVeil veil);
}