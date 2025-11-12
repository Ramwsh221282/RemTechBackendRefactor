using System.Reflection;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Persistence.NpgSql;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Application.EventListeners;

public static class IdentityEventListenersModule
{
    private static readonly Assembly Assembly = typeof(IdentityEventListenersModule).Assembly;
    
    public static AsyncSubjectNotificationHandle<IdentitySubjectRegisteredNotification> OnRegistered(NpgSqlIdentitySubjectCommands commands)
    {
        return async (@event, ct) =>
        {
            IdentitySubjectSnapshot snapshot = @event.Snapshot;
            if (await commands.IsEmailUnique(snapshot.Email, ct)) return Conflict($"Почта: {snapshot.Email} занята.");
            if (await commands.IsLoginUnique(snapshot.Login, ct)) return Conflict($"Логин: {snapshot.Login} занята.");
            Result<Unit> inserted = await commands.Insert(Subject.Create(snapshot), ct);
            return inserted.IsFailure ? inserted.Error : Unit.Value;
        };
    }

    extension(IServiceCollection services)
    {
        public void RegisterEventListeners()
        {
            services.AddScopedDelegate<AsyncSubjectNotificationHandle<IdentitySubjectRegisteredNotification>>(Assembly);
        }

        public void RegisterEventsRegistry()
        {
            services.AddScoped<SubjectEventsRegistry>(sp =>
            {
                SubjectEventsRegistry registry = new();
                return registry.WithEventHandlerOf<IdentitySubjectRegisteredNotification>(sp);
            });
        }
    }

    extension(SubjectEventsRegistry reg)
    {
        private SubjectEventsRegistry WithEventHandlerOf<TEvent>(IServiceProvider sp) 
            where TEvent : IdentitySubjectNotification
        {
            AsyncSubjectNotificationHandle<TEvent> handler = sp.Resolve<AsyncSubjectNotificationHandle<TEvent>>();
            return reg.AddHandlerFor(handler);
        }
    }
}