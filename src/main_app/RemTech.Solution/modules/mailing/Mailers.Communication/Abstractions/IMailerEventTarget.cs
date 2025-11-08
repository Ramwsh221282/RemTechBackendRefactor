namespace Mailers.Communication.Abstractions;

public interface IMailerEventTarget
{
    void Listen(IMailerEventSource source);
    void Accept(Optional<Guid> id, Optional<string> email, Optional<string> password, Optional<int> sendLimit, Optional<int> sendCurrent);
    void Notify(MailerEvent @event);
}