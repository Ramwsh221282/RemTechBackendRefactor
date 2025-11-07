namespace Mailing.Moduled.Contracts;

public interface IEmailMessage
{
    Task Send(string to, string subject, string body, CancellationToken ct = default);
}