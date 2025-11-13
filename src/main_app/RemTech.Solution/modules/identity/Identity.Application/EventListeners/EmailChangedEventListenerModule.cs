using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Persistence.NpgSql;
using Identity.Persistence.NpgSql.SubjectsModule;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Application.EventListeners;

public static class EmailChangedEventListenerModule
{
    private static AsyncNotificationHandle<EmailChanged> Loggable(
        AsyncNotificationHandle<EmailChanged> origin,
        Serilog.ILogger logger
    ) => async (@event, ct) =>
    {
        SubjectSnapshot snap = @event.Snapshot;
        Result<Unit> result = await origin(@event, ct);
        if (result.IsFailure)
            logger.Error(
                """
                Id: {Id}
                Login: {Login}
                Изменил почту на: {Email}
                Успешно: {Success}
                Ошибка: {Error}
                """,
                snap.Id,
                snap.Login,
                snap.Email,
                result.IsSuccess,
                result.Error.Message);
        if (result.IsSuccess)
            logger.Error(
                """
                Id: {Id}
                Login: {Login}
                Изменил почту на: {Email}
                Успешно: {Success}
                """,
                snap.Id,
                snap.Login,
                snap.Email,
                result.IsSuccess);
        return result;
    };
    
    private static AsyncNotificationHandle<EmailChanged> Transactional(
        AsyncNotificationHandle<EmailChanged> origin,
        NpgSqlIdentityCommands commands) =>
        async (@event, ct) =>
            await commands.Session.ExecuteUnderTransaction(async () => await origin(@event, ct), ct);
    
    private static AsyncNotificationHandle<EmailChanged> OnEmailChanged(NpgSqlSubjectCommands commands) =>
        async (@event, ct) =>
        {
            SubjectSnapshot snap = @event.Snapshot;
            if (!await commands.IsEmailUnique(snap.Email, ct))
                return Conflict($"Почта: {snap.Email} уже занята.");
            
            SubjectQueryArgs args = new(Id: snap.Id, WithLock: true);
            Optional<Subject> subject = await commands.FindSingle(args, ct);
            if (subject.NoValue) 
                return NotFound("Учетная запись не найдена.");

            Result<Unit> update = await commands.Update(Subject.Create(snap), ct);
            return update.IsFailure ? update.Error : Unit.Value;
        };

    extension(IServiceCollection services)
    {
        public void AddOnEmailChangedListener()
        {
            services.AddScoped<AsyncNotificationHandle<EmailChanged>>(sp =>
            {
                NpgSqlIdentityCommands commands = sp.Resolve<NpgSqlIdentityCommands>();
                Serilog.ILogger logger = sp.Resolve<Serilog.ILogger>();
                AsyncNotificationHandle<EmailChanged> core = OnEmailChanged(commands.Subjects);
                AsyncNotificationHandle<EmailChanged> txn = Transactional(core, commands);
                AsyncNotificationHandle<EmailChanged> log = Loggable(txn, logger);
                return log;
            });
        }
    }

    extension(NotificationsRegistry reg)
    {
        public NotificationsRegistry WithEmailChangedEventListener(IServiceProvider sp)
        {
            return reg.AddNotificationHandler(sp.Resolve<AsyncNotificationHandle<EmailChanged>>());
        }
    }
}