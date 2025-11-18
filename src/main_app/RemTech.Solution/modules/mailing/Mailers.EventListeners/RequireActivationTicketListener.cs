using System.Text;
using Mailers.Application.Features.SendEmailMessage;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.RabbitMq.Abstractions;

namespace Mailers.EventListeners;

public sealed class RequireActivationTicketListener
    (
        RabbitMqConnectionSource connectionSource, 
        IServiceProvider sp)
    : RabbitMqListenerHostService(connectionSource)
{
    protected override AsyncEventHandler<BasicDeliverEventArgs> GetConsumerMethod() =>
        async (sender, @event) =>
        {
            string json = Encoding.UTF8.GetString(@event.Body.Span);
            await using AsyncServiceScope scope = sp.CreateAsyncScope(); 
            SendEmailMessageUseCase sendMessage = scope.ServiceProvider.Resolve<SendEmailMessageUseCase>();
            await Task.Yield();
        };

    protected override Func<RabbitMqConnectionSource, CancellationToken, Task<RabbitMqListener>> GetListener()
    {
        return async (source, ct) => await source.CreateListener("mailings", "require.activation", ct);;
    }
}