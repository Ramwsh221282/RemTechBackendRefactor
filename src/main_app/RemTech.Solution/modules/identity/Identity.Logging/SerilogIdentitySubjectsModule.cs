using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class SerilogIdentitySubjectsModule
{
    public static AsyncSubjectNotificationHandle<IdentitySubjectRegisteredNotification> OnRegistered(
        ILogger logger,
        AsyncSubjectNotificationHandle<IdentitySubjectRegisteredNotification> origin) =>
        (async (@event, ct) => await origin.Loggable(logger, @event, ct));

    extension<TEvent>(AsyncSubjectNotificationHandle<TEvent> handle) where TEvent : IdentitySubjectNotification
    {
        private async Task<Result<Unit>> Loggable(ILogger logger, TEvent @event, CancellationToken ct)
        {
            string eventName = typeof(TEvent).Name;
            logger.Information("Handling event: {EventName}", eventName);
        
            Result<Unit> result = await handle(@event, ct);
            if (result.IsFailure)
            {
                logger.Error("{EventName} failure. Error: {Error}.", eventName, result.Error.Message);
                return result;
            }
        
            logger.Information("Handled event: {EventName}", eventName);
            return Unit.Value;
        }
    }
}