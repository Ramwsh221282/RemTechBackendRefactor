using RabbitMQ.Client.Events;
using RemTech.RabbitMq.Abstractions;

namespace Tickets.EventListeners;

public sealed class TicketCreatedEventListener : RabbitMqListenerHostService
{
    private const string Queue = "tickets";
    private const string Exchange = "tickets";
    private const string RoutingKey = "tickets.create";
    private const string ExchangeType = "topic";
    private IServiceProvider _sp;
    
    protected override AsyncEventHandler<BasicDeliverEventArgs> GetConsumerMethod(RabbitMqListener listener)
    {
        return async (sender, @event) =>
        {
            JsonMessageFromRabbitMq message = new(@event.Body);
            TicketRequiredMessage ticketRequired = TicketRequiredMessage.FromRabbitMqJson(message);
            await ticketRequired.Register(_sp);
            await listener.Acknowledge(@event);
            Console.WriteLine($"{nameof(TicketCreatedEventListener)} acknowledged");
        };
    }

    protected override Func<RabbitMqConnectionSource, CancellationToken, Task<RabbitMqListener>> GetListener()
    {
        return async (source, token) =>
        {
            DeclareQueueArgs args = new(Queue, Exchange, RoutingKey, ExchangeType);
            return await source.CreateListener(args, token);
        };
    }
    
    public TicketCreatedEventListener(IServiceProvider sp, RabbitMqConnectionSource connectionSource) 
        : base(connectionSource)
    {
        _sp = sp;
    }
}