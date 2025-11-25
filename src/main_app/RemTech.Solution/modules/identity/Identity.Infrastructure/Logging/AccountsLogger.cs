using Identity.Core;
using Identity.Core.Accounts.Events;

namespace Identity.Infrastructure.Logging;

public sealed class AccountsLogger(Serilog.ILogger logger) :
    IEventHandler<AccountRegisteredEvent>,
    IEventHandler<AccountEmailChangedEvent>,
    IEventHandler<AccountPasswordChangedEvent>
{
    private readonly Queue<Action<Serilog.ILogger>> _logActions = [];

    public void ProcessLogging()
    {
        while (_logActions.Count > 0)
        {
            Action<Serilog.ILogger> action = _logActions.Dequeue();
            action(logger);
        }
    }
    
    public void ReactOnEvent(AccountRegisteredEvent @event)
    {
        _logActions.Enqueue(l =>
        {
            object[] properties = [@event.Email, @event.Id, @event.Name];
            l.Information("""
                          Создана учетная запись:
                          Email: {Email}
                          Id: {Id}
                          Name: {Name}
                          """, properties);
        });
    }

    public void ReactOnEvent(AccountEmailChangedEvent @event)
    {
        _logActions.Enqueue(l =>
        {
            object[] properties = [@event.Id, @event.OldEmail, @event.NewEmail];
            l.Information("""
                               Учетная запись переименована:
                               Id: {Id}
                               Email: {Email}
                               Name: {Name}
                               """, properties);
        });
    }

    public void ReactOnEvent(AccountPasswordChangedEvent @event)
    {
        _logActions.Enqueue(l =>
        {
            l.Information("Изменен пароль у учетной записи: {Id}.", @event.Id);
        });
    }
}