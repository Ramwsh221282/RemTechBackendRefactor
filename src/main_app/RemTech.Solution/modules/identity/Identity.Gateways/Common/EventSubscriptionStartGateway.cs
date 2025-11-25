using Identity.Core;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Common;

public sealed class EventSubscriptionStartGateway<TRequest, TResponse>(
    IGateway<TRequest, TResponse> origin,
    EventsStore events)
    : IGateway<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResponse
{
    public async Task<Result<TResponse>> Execute(TRequest request)
    {
        events.SubscribeByObjectFields(origin);
        return await origin.Execute(request);
    }
}