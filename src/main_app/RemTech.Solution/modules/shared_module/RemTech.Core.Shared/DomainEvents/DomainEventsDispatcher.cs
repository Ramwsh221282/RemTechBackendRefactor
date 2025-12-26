using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Result;
using Serilog;

namespace RemTech.Core.Shared.DomainEvents;

public sealed class DomainEventsDispatcher(IServiceProvider sp) : IDomainEventsDispatcher
{
    private static readonly Type HandlerType = typeof(IDomainEventHandler<>);
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

    public async Task<Status> Dispatch(
        IEnumerable<IDomainEvent> events,
        CancellationToken ct = default
    )
    {
        foreach (IDomainEvent @event in events)
        {
            Type genericHandler = HandlerType.MakeGenericType(@event.GetType());
            await using AsyncServiceScope scope = sp.CreateAsyncScope();
            IEnumerable<object> handlers = GetHandlers(scope, genericHandler);

            foreach (object handler in handlers)
            {
                Status result = await Handle(handler, genericHandler, @event, ct);
                if (result.IsFailure)
                    return result;
            }
        }

        return Status.Success();
    }

    private IEnumerable<object> GetHandlers(AsyncServiceScope scope, Type genericHandler) =>
        (IEnumerable<object>)
            scope.ServiceProvider.GetService(
                typeof(IEnumerable<>).MakeGenericType(genericHandler)
            )!;

    private async Task<Status> Handle(
        object handler,
        Type handlerType,
        IDomainEvent @event,
        CancellationToken ct
    )
    {
        Logger.Information(
            "Handling domain event: {EventName} by {Handler}...",
            @event.GetType().Name,
            handler.GetType().Name
        );

        Status result = await (Task<Status>)
            handlerType.GetMethod("Handle")!.Invoke(handler, [@event, ct])!;

        if (result.IsFailure)
        {
            Logger.Information(
                "Handled {EventName} by {Handler}. Error: {Error}",
                @event.GetType().Name,
                handler.GetType().Name,
                result.Error.ErrorText
            );

            return result;
        }

        Logger.Information(
            "Handled {EventName} by {Handler}.",
            @event.GetType().Name,
            handler.GetType().Name
        );

        return result;
    }
}
