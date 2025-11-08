namespace Mailers.Core.MailersContext;

public sealed class Mailer(MailerMetadata metadata, MailerStatistics statistics) : IMailerEventSource
{
    private readonly List<IMailerEventTarget> _eventTargets = [];
    
    public void Register()
    {
        var @event = new MailerEvent();
        metadata.SignRegistration(@event);
        statistics.SignRegistration(@event);
        Notify(@event);
    }

    public void Delete()
    {
        var @event = new MailerEvent();
        metadata.SignRegistration(@event);
        Notify(@event);
    }

    public void Accept(IMailerEventTarget target)
    {
        if (_eventTargets.Contains(target)) return;
        _eventTargets.Add(target);
    }

    private void Notify(MailerEvent @event)
    {
        foreach (var target in _eventTargets)
            target.Notify(@event);
    }
    
    public static Result<Mailer> Create(Result<MailerMetadata> metadata, Result<MailerStatistics> statistics, IMailerEventTarget? target = null)
    {
        if (metadata.IsFailure) return Validation(metadata);
        if (statistics.IsFailure) return Validation(statistics);
        var mailer = new Mailer(metadata.Value, statistics.Value);
        return mailer;
    }
    
    public static Result<Mailer> Create(Result<MailerMetadata> metadata)
    {
        if (metadata.IsFailure) return Validation(metadata);
        var statistics = new MailerStatistics();
        var mailer = new Mailer(metadata.Value, statistics);
        return mailer;
    }
}