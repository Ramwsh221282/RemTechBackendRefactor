// using System.Text.Json;
// using Microsoft.Extensions.DependencyInjection;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using RemTech.Core.Shared.Result;
// using Remtech.Infrastructure.RabbitMQ.Consumers;
// using RemTech.UseCases.Shared.Cqrs;
// using Telemetry.Contracts;
// using Telemetry.Domain.Models;
// using Telemetry.Domain.TelemetryContext;
// using Telemetry.Domain.UseCases.AddAction;
//
// namespace Telemetry.Infrastructure.RabbitMQ;
//
// public sealed class ActionInvokedEventListener : BaseExchangedRabbitMqListener
// {
//     public const string QueueName = "telemetry-action-invoked";
//     public const string ExchangeName = "telemetry-exchange";
//     private const string Context = nameof(ActionInvokedEventListener);
//     private readonly IServiceScopeFactory _factory;
//     private readonly Serilog.ILogger _logger;
//
//     public ActionInvokedEventListener(
//         RabbitMqConnectionProvider provider,
//         IServiceScopeFactory factory,
//         Serilog.ILogger logger
//     )
//         : base(provider)
//     {
//         _factory = factory;
//         _logger = logger;
//     }
//
//     public override void Configure()
//     {
//         // настройка очереди в брокере.
//         Configurer.Queue.WithName(QueueName);
//         Configurer.Exchange.WithName(ExchangeName).WithType(ExchangeType.Direct);
//         _logger.Information("{Context} configured.", Context);
//     }
//
//     public override async Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs)
//     {
//         try
//         {
//             byte[] bodyData = eventArgs.Body.ToArray();
//             var @event = JsonSerializer.Deserialize<SaveActionInfoEvent>(bodyData);
//
//             if (@event == null)
//             {
//                 _logger.Warning("{Context} invalid event info.", Context);
//                 await Acknowledge(eventArgs);
//                 return;
//             }
//
//             Status<ActionRecord> result = await SaveRecord(@event);
//             await Acknowledge(eventArgs);
//             _logger.Information("{Context} handled event.", Context);
//             if (result.IsFailure)
//                 _logger.Error("{Context} error: {Error}", Context, result.Error.ErrorText);
//         }
//         catch (Exception ex)
//         {
//             _logger.Fatal("{Context} error: {Error}", Context, ex);
//         }
//     }
//
//     private async Task<Status<ActionRecord>> SaveRecord(SaveActionInfoEvent @event)
//     {
//         await using AsyncServiceScope scope = _factory.CreateAsyncScope();
//         IBCommandHandler<AddActionCommand, ActionRecord> handler =
//             scope.ServiceProvider.GetRequiredService<
//                 IBCommandHandler<AddActionCommand, ActionRecord>
//             >();
//         return await handler.Handle(EventToCommand(@event));
//     }
//
//     private AddActionCommand EventToCommand(SaveActionInfoEvent @event)
//     {
//         return new AddActionCommand(
//             @event.Comments,
//             @event.Name,
//             @event.Status,
//             @event.InvokerId,
//             @event.OccuredAt
//         );
//     }
// }
