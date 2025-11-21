using RabbitMQ.Client.Events;
using RemTech.Functional.Extensions;
using RemTech.RabbitMq.Abstractions;
using Tickets.Core;

namespace Tickets.EventListeners.Points.TicketCreated;

public sealed class TicketCreatedEventListener(
    RabbitMqConnectionSource connectionSource, 
    IServiceProvider sp, 
    Serilog.ILogger logger)
    : RabbitMqListenerHostService(connectionSource)
{
    private const string Queue = "tickets";
    private const string Exchange = "tickets";
    private const string RoutingKey = "tickets.create";
    private const string ExchangeType = "topic";
    private const string MessageName = nameof(TicketCreatedEventListener);
    private const string Context = nameof(TicketCreatedEventListener);

    private readonly AsyncEventHandler<BasicDeliverEventArgs> _handler = async (_, @event) =>
    {
        try
        {
            JsonMessageFromRabbitMq message = new(@event.Body);
            TicketRequiredMessage ticketRequired = TicketRequiredMessage.FromRabbitMqJson(message);
            Result<Ticket> result = await ticketRequired.Register(sp);
            object[] logArgs = [MessageName, result.IsSuccess, result.Error.Message];
            
            if (result.IsSuccess)
                logger.Information("{Message} handled {Success}", logArgs);
            if (result.IsFailure)
                logger.Information("{Message} handled {Success}. Error: {Error}", logArgs);
        }
        catch(Exception ex)
        {
            logger.Error("{Context} failed.", Context);
            logger.Error("{Message}", ex);
        }
    };

    protected override Func<RabbitMqConnectionSource, CancellationToken, Task<RabbitMqListener>> GetListener()
    {
        return async (source, token) =>
        {
            DeclareQueueArgs args = new(Queue, Exchange, RoutingKey, ExchangeType);
            return await source.CreateListener(_handler, args, token);
        };
    }
}