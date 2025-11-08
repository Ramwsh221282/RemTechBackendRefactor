namespace Mailers.Communication.Abstractions;

public interface IMailerEventSource
{
    public void Accept(IMailerEventTarget target);
}