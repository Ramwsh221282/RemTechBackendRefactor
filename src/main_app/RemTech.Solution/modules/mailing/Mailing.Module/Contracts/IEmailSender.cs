namespace Mailing.Module.Contracts;

public interface IEmailSender
{
    IEmailMessage FormEmailMessage();
    EmailSenderOutput Print();
    Task<bool> Save(IEmailSendersSource source, CancellationToken ct = default);
    Task<bool> Remove(IEmailSendersSource source, CancellationToken ct = default);
}
