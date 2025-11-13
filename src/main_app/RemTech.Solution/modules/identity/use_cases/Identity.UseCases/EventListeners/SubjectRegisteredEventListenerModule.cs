// using Identity.Core.SubjectsModule.Contracts;
// using Identity.Core.SubjectsModule.Models;
// using Identity.Core.SubjectsModule.Modules;
// using Identity.Core.SubjectsModule.Notifications;
// using Identity.Core.SubjectsModule.Notifications.Abstractions;
// using Identity.Persistence.NpgSql.SubjectsModule;
// using Microsoft.Extensions.DependencyInjection;
// using RemTech.BuildingBlocks.DependencyInjection;
// using RemTech.Functional.Extensions;
//
// namespace Identity.UseCases.EventListeners;
//
// public static class SubjectRegisteredEventListenerModule
// {
//     private static AsyncNotificationHandle<Registered> OnRegisteredWithPasswordHash(
//         AsyncNotificationHandle<Registered> origin,
//         HashPassword hash) => (@event, ct) =>
//     {
//         SubjectSnapshot snap = @event.Snapshot;
//         SubjectSnapshot withHashed = snap with { Password = hash(snap.Password) };
//         Registered withHashedSnap = @event with { Snapshot = withHashed };
//         return origin(withHashedSnap, ct);
//     };
//
//     private static AsyncNotificationHandle<Registered> OnRegisteredWithLogging(
//         AsyncNotificationHandle<Registered> origin,
//         Serilog.ILogger logger) => async (@event, ct) =>
//     {
//         Result<Unit> result = await origin(@event, ct);
//         ResolveLogAction(logger, @event, result).Invoke();
//         return result;
//     };
//
//     private static Action ResolveLogAction(Serilog.ILogger logger, Registered @event, Result<Unit> result)
//     {
//         return result.IsSuccess switch
//         {
//             true => () => { SuccessUserRegistration().Invoke(logger, @event.Snapshot); },
//             false => () => { FailureUserRegistration().Invoke(logger, @event.Snapshot, result.Error); }
//         };
//     }
//
//     private static Action<Serilog.ILogger, SubjectSnapshot> SuccessUserRegistration() =>
//         (logger, snapshot) =>
//         {
//             logger.Information(
//                 """
//                 Email: {Email}
//                 Login: {Login}
//                 Password: {Password}
//                 ID: {Id}
//                 Зарегистрирован: {Registered}
//                 """, 
//                 snapshot.Email, 
//                 snapshot.Login, 
//                 snapshot.Password, 
//                 snapshot.Id, 
//                 true);
//         };
//
//     private static Action<Serilog.ILogger, SubjectSnapshot, Error> FailureUserRegistration() =>
//         (logger, snapshot, error) =>
//         {
//             logger.Information(
//                 """
//                 Email: {Email}
//                 Login: {Login}
//                 Password: {Password}
//                 ID: {Id}
//                 Зарегистрирован: {Registered}
//                 Ошибка: {Error}
//                 """, 
//                 snapshot.Email, 
//                 snapshot.Login, 
//                 snapshot.Password, 
//                 snapshot.Id, 
//                 true,
//                 error.Message);
//         };
//     
//     private static AsyncNotificationHandle<Registered> OnRegistered(NpgSqlSubjectCommands commands) =>
//         async (@event, ct) =>
//         {
//             SubjectSnapshot snap = @event.Snapshot;
//             if (await commands.IsEmailUnique(snap.Email, ct)) return Conflict($"Почта: {snap.Email} занята.");
//             if (await commands.IsLoginUnique(snap.Login, ct)) return Conflict($"Логин: {snap.Login} занята.");
//             Result<Unit> inserted = await commands.Insert(Subject.Create(snap), ct);
//             return inserted.IsFailure ? inserted.Error : Unit.Value;
//         };
//
//     extension(IServiceCollection services)
//     {
//         public void AddSubjectRegisteredEventListener()
//         {
//             services.AddScoped<AsyncNotificationHandle<Registered>>(sp =>
//             {
//                 NpgSqlSubjectCommands commands = sp.Resolve<NpgSqlSubjectCommands>();
//                 HashPassword hash = sp.Resolve<HashPassword>();
//                 Serilog.ILogger logger = sp.Resolve<Serilog.ILogger>();
//                 AsyncNotificationHandle<Registered> core = OnRegistered(commands);
//                 AsyncNotificationHandle<Registered> withHash = OnRegisteredWithPasswordHash(core, hash);
//                 AsyncNotificationHandle<Registered> withLog = OnRegisteredWithLogging(withHash, logger);
//                 return withLog;
//             });
//         }
//     }
//
//     extension(NotificationsRegistry registry)
//     {
//         public NotificationsRegistry WithSubjectRegisteredEventListener(IServiceProvider sp)
//         {
//             return registry.AddNotificationHandler(sp.Resolve<AsyncNotificationHandle<Registered>>());
//         }
//     }
// }