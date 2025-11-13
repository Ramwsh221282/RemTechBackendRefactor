// using Identity.Core.SubjectsModule.Contracts;
// using Identity.Core.SubjectsModule.Models;
// using Identity.Core.SubjectsModule.Notifications;
// using Identity.Core.SubjectsModule.Notifications.Abstractions;
// using Identity.Persistence.NpgSql;
// using Identity.Persistence.NpgSql.SubjectsModule;
// using Microsoft.Extensions.DependencyInjection;
// using RemTech.BuildingBlocks.DependencyInjection;
// using RemTech.Functional.Extensions;
//
// namespace Identity.UseCases.EventListeners;
//
// public static class PasswordChangedEventListenerModule
// {
//     private static AsyncNotificationHandle<PasswordChanged> Logging(
//         AsyncNotificationHandle<PasswordChanged> origin,
//         Serilog.ILogger logger) => async (@event, ct) =>
//     {
//         SubjectSnapshot snapshot = @event.Snapshot;
//         Result<Unit> changing = await origin(@event, ct);
//         if (changing.IsFailure)
//             logger.Error(
//                 """
//                 Email: {Email},
//                 Login: {Login},
//                 Изменил пароль: {Changed}          
//                 Ошибка: {Error}
//                 """, 
//                 snapshot.Email,
//                 snapshot.Login,
//                 false,
//                 changing.Error.Message);
//         if (changing.IsSuccess)
//             logger.Error(
//                 """
//                 Email: {Email},
//                 Login: {Login},
//                 Изменил пароль: {Changed}          
//                 """, 
//                 snapshot.Email,
//                 snapshot.Login,
//                 true);
//         return changing;
//     };
//     
//     private static AsyncNotificationHandle<PasswordChanged> Transactional(
//         AsyncNotificationHandle<PasswordChanged> origin,
//         NpgSqlIdentityCommands commands) => async (@event, ct) =>
//         await commands.Session.ExecuteUnderTransaction<Unit>(async () =>
//         {
//             Result<Unit> handled = await origin(@event, ct);
//             return handled.IsFailure ? handled.Error : Unit.Value;
//         }, ct);
//     
//     private static AsyncNotificationHandle<PasswordChanged> OnPasswordChanged(NpgSqlSubjectCommands commands) =>
//         async (@event, ct) =>
//         {
//             SubjectSnapshot snapshot = @event.Snapshot;
//             SubjectQueryArgs args = new(Id: snapshot.Id, WithLock: true);
//             Optional<Subject> subject = await commands.FindSingle(args, ct);
//             if (subject.NoValue) return NotFound("Учетная запись не найдена.");
//             Result<Unit> update = await commands.Update(subject.Value, ct);
//             return update.IsFailure ? update.Error : Unit.Value;
//         };
//
//     extension(IServiceCollection services)
//     {
//         public void AddPasswordChangedEventListener()
//         {
//             services.AddScoped<AsyncNotificationHandle<PasswordChanged>>(sp =>
//             {
//                 NpgSqlIdentityCommands npgSql = sp.Resolve<NpgSqlIdentityCommands>();
//                 Serilog.ILogger logger = sp.Resolve<Serilog.ILogger>();
//                 AsyncNotificationHandle<PasswordChanged> core = OnPasswordChanged(npgSql.Subjects);
//                 AsyncNotificationHandle<PasswordChanged> txn = Transactional(core, npgSql);
//                 AsyncNotificationHandle<PasswordChanged> log = Logging(txn, logger);
//                 return log;
//             });
//         }
//     }
//
//     extension(NotificationsRegistry registry)
//     {
//         public NotificationsRegistry WithOnPasswordChangedListener(IServiceProvider sp)
//         {
//             return registry.AddNotificationHandler(sp.Resolve<AsyncNotificationHandle<PasswordChanged>>());
//         }
//     }
// }