// using Identity.Core.SubjectsModule.Notifications.Abstractions;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Identity.UseCases.EventListeners;
//
// public static class IdentityEventListenersModule
// {
//     extension(IServiceCollection services)
//     {
//         public void RegisterEventListeners()
//         {
//             services.AddSubjectRegisteredEventListener();
//             services.AddPasswordChangedEventListener();
//             services.AddOnEmailChangedListener();
//         }
//         
//         public void RegisterEventsRegistry()
//         {
//             services.AddScoped<NotificationsRegistry>(sp => new NotificationsRegistry()
//                 .WithOnPasswordChangedListener(sp)
//                 .WithSubjectRegisteredEventListener(sp)
//                 .WithEmailChangedEventListener(sp));
//         }
//     }
// }